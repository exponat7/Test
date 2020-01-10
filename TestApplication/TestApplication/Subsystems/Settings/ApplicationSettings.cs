using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TestApplication.Subsystems.Settings
{
    public class ApplicationSettings
    {
        //Все диапазоны и задержки в милисекундах
        public int Tm { get; private set; }
        public int Td { get; private set; }
        public int FinalizeRangeFrom { get; private set; }
        public int FinalizeRangeTo { get; private set; }


        public ApplicationSettings(IConfiguration Configuration)
        {
            try { Tm = Configuration.GetValue<int>("ExpirationTimeouts:Tm"); } catch { Tm = 20000; }
            try { Td = Configuration.GetValue<int>("ExpirationTimeouts:Td"); } catch { Td = 60000; }
            //
            try { FinalizeRangeFrom = Configuration.GetValue<int>("FinalizationRange:From"); } catch { FinalizeRangeFrom = 30000; }
            try { FinalizeRangeTo = Configuration.GetValue<int>("FinalizationRange:To"); } catch { FinalizeRangeTo = 90000; }
            //Предотвращаем потенциальную ошибку
            if(FinalizeRangeFrom> FinalizeRangeTo)
            {
                var tmp = FinalizeRangeFrom;
                FinalizeRangeFrom = FinalizeRangeTo;
                FinalizeRangeTo = tmp;
            }
        }

        /// <summary>
        /// Сохранение настроек
        /// </summary>
        /// <param name=""></param>
        public void Save(int Tm, int Td, int finalizeRangeFrom, int finalizeRangeTo)
        {
            this.Tm = Tm;
            this.Td = Td;
            FinalizeRangeFrom = finalizeRangeFrom;
            FinalizeRangeTo = finalizeRangeTo;
            //Теперь в конфиг
            Startup.Configuration["ExpirationTimeouts:Tm"] = Tm.ToString();
            Startup.Configuration["ExpirationTimeouts:Td"] = Td.ToString();
            Startup.Configuration["FinalizationRange:From"] = finalizeRangeFrom.ToString();
            Startup.Configuration["FinalizationRange:To"] = finalizeRangeTo.ToString();
            //Теперь в файл
            JObject apps = JObject.Parse(File.ReadAllText("appsettings.json"));
            //
            var tm = apps.Descendants().OfType<JProperty>().Where(w => w.Name == "Tm").SingleOrDefault();
            if (tm != null) tm.Value = Tm;
            var td = apps.Descendants().OfType<JProperty>().Where(w => w.Name == "Td").SingleOrDefault();
            if (td != null) td.Value = Td;
            var from = apps.Descendants().OfType<JProperty>().Where(w => w.Name == "From").SingleOrDefault();
            if (from != null) from.Value = finalizeRangeFrom;
            var to = apps.Descendants().OfType<JProperty>().Where(w => w.Name == "To").SingleOrDefault();
            if (to != null) to.Value = finalizeRangeTo;
            //
            File.WriteAllText("appsettings.json", apps.ToString());
        }
    }
}
