using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using GONOrderingSystems.Logic.Managers.Interface;
using GONOrderingSystems.Logic.Managers;
using GONOrderingSystems.Common.Providers.Interface;
using GONOrderingSystems.Api.Formatters;
using Microsoft.Net.Http.Headers;
using GONOrderingSystems.Common.DataModels;
using GONOrderingSystems.Common.Providers.Implementation;
using AutoMapper;
using GONOrderingSystems.Api.Model;

namespace GONOrderingSystems.Apis
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            Mapper.Initialize(cfg => {
                cfg.CreateMap<SubmitOrderDto, Order>();
                cfg.CreateMap<CommitOrderDto, Order>();
            });
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression();

            services.AddSingleton<IDeadlineManager, DeadlineManager>();
            services.AddSingleton<IOrderManager, OrderManager>();
            services.AddSingleton<ILogProvider, LogProvider>(s => new LogProvider(Configuration["GraylogSettings:Host"].ToString()
            , Int32.Parse(Configuration["GraylogSettings:Port"].ToString())));

            services.AddSingleton<IDataProvider, MongoDBProvider>(settings => new MongoDBProvider(
                Configuration["MongoDbSettings:Host"].ToString(),
                Configuration["MongoDbSettings:Database"].ToString(),
                Configuration["MongoDbSettings:OrderCollection"].ToString()
                )

            );
            services.AddSingleton<IPubSubProvider, KafkaProvider>();
            services.AddSingleton<IMetricsProvider, PrometheusProvider>();

            services.Configure<KafkaSettings>(Configuration.GetSection("KafkaSettings"));
            services.Configure<GraylogSettings>(Configuration.GetSection("GraylogSettings"));
            services.Configure<MongoDbSettings>(Configuration.GetSection("MongoDbSettings"));
            services.AddOptions();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Gon Ordering System Api", Version = "v1" });
            });

            services.AddMvc();

            services.AddMvc(options =>
            {
                options.InputFormatters.Add(new ProtobufInputFormatter());
                options.OutputFormatters.Add(new ProtobufOutputFormatter());
                options.FormatterMappings.SetMediaTypeMappingForFormat("protobuf", MediaTypeHeaderValue.Parse("application/x-protobuf"));
                options.FormatterMappings.SetMediaTypeMappingForFormat("js", MediaTypeHeaderValue.Parse("application/json"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseResponseCompression();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gon Ordering System Api V1");
            });
        }
    }
}
