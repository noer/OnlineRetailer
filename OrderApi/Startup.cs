using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrderApi.Data;
using OrderApi.Infrastructure;
using SharedModels;
using Swashbuckle.AspNetCore.Swagger;
using Order = OrderApi.Models.Order;

namespace OrderApi
{
    public class Startup
    {
        Uri productServiceBaseUrl = new Uri("http://productapi/api/products/");
        Uri customerServiceBaseUrl = new Uri("http://customerapi/api/customer/");

        private string cloudAMQPConnectionString =
            "host=bear.rmq.cloudamqp.com;virtualHost=vwwmjixz;username=vwwmjixz;password=JvUd-WcdNGkmCaBQVUxykmM2NEb6R3nc";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Sqlite database:
            services.AddDbContext<OrderApiContext>(opt => opt.UseSqlite("Data Source=orders.db"));

            // Register repositories for dependency injection
            services.AddScoped<IRepository<Order>, OrderRepository>();

            // Register database initializer for dependency injection
            services.AddTransient<IDbInitializer, DbInitializer>();

            // Register product service gateway for dependency injection
            services.AddSingleton<IServiceGateway<ProductDTO>>(new
                ProductServiceGateway(productServiceBaseUrl));

            // Register customer service gateway for dependency injection
            services.AddSingleton<IServiceGateway<CustomerDTO>>(new
                CustomerServiceGateway(customerServiceBaseUrl));

            // Register mail service gateway for dependency injection
            services.AddSingleton<IMailService>(new MailService());

            // Register MessagePublisher (a messaging gateway) for dependency injection
            services.AddSingleton<IMessagePublisher>(new
                MessagePublisher(cloudAMQPConnectionString));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).
                AddJsonOptions(options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "OrderAPI", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Initialize the database
            using (var scope = app.ApplicationServices.CreateScope())
            {
                // Initialize the database
                var services = scope.ServiceProvider;
                var dbContext = services.GetService<OrderApiContext>();
                var dbInitializer = services.GetService<IDbInitializer>();
                dbInitializer.Initialize(dbContext);
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1");
            });
            
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
