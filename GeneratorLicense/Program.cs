using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeneratorLicense
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Генератор лицензий");
            string del = new string('-', 30);
            Console.WriteLine(del);

            LicenseDto lic = new LicenseDto();

            Console.Write("Организация/частное лицо: ");
            lic.LicenseeName = Console.ReadLine();

            Console.Write("Срок действия (день.месяц.год): ");

            var date = Console.ReadLine().Split(new char[] { '.' });

            lic.ValidUntil = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0]));

            Console.WriteLine("Доступные функции системы");
            Console.WriteLine("Включить возможность сохранить данные в файл? Y/N");
            string answer = Console.ReadLine();

            switch (answer)
            {
                case "Y":
                    lic.AllowedFeatures.Add("AllowSaveInFile");
                    break;
            }

            Console.WriteLine("Включить возможность загрузить данные из файла? Y/N");
            answer = Console.ReadLine();

            switch (answer)
            {
                case "Y":
                    lic.AllowedFeatures.Add("AllowReadFromFile");
                    break;
            }

            GeneratorManager manager = new GeneratorManager();
            manager.GenerateNewKeyPair();

            manager.CreateLicenseFile(lic, "license.xml");

            Console.WriteLine("Файл лицензии успешно создан!");

            Console.ReadKey();
        }
    }
}
