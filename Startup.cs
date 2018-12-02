using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ApiFindJson
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

 /*  **Цели и задачи**
Создать HTTP API для поиска по JSON-документам в папках произвольной степени вложенности

**Функциональные требования**
Сервис должен предоставлять API для поиска по протоколу HTTP

Поиск должен осуществляться по имени поля ( наличию ), его значению ( или одному из значений, в случае массива ) до третьего уровня вложенности.
Результатом поиска должен быть JSON-документ, который содержит: количество совпадений, путь к файлам для каждого совпадения, найденное совпадение.
Поисковый запрос должен поддерживать конъюнкцию и дизъюнкцию по любому количеству полей или их значений.
При успешном поиске должен возвращаться код ответа HTTP 200 OK, при отсутствии данных - 404 Not found, при ошибке - 503 Internal Error
Аутентификация и авторизация не требуются

**Нефунциональные требования**
Сервис должен быть выполнен в виде единого пакета ( rpm, egg ) или исполняемого файла
Сервис должен иметь возможность настройки следующих параметров:
- Номер порта TCP для приема входящих соединений
- Путь к корневой папке ( папкам ) с JSON-файлами
- Количество потоков / рутин / размер очереди и другие необходимые для улучшения производительности параметры
Тестовый запрос на контрольных данных должен работать не медленее, чем 0.3s. Контрольные данные могут быть предоставлены по запросу.
 */
