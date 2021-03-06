﻿using System.Threading.Tasks;
using GraphiQl;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Data;
using ProductApi.Infrastructure;
using ProductApi.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace ProductApi
{
    public class Startup
    {
        readonly string cloudAMQPConnectionString = 
            "host=bear.rmq.cloudamqp.com;virtualHost=vwwmjixz;username=vwwmjixz;password=JvUd-WcdNGkmCaBQVUxykmM2NEb6R3nc";
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // In-memory database:
            //services.AddDbContext<ProductApiContext>(opt => opt.UseInMemoryDatabase("ProductsDb"));
            services.AddDbContext<ProductApiContext>(opt => opt.UseSqlite("Data Source=products.db"));

            // Register repositories for dependency injection
            services.AddScoped<IRepository<Product>, ProductRepository>();

            // Register database initializer for dependency injection
            services.AddTransient<IDbInitializer, DbInitializer>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();
            services.AddSingleton<ProductQuery>();
            services.AddSingleton<ProductMutation>();
            services.AddSingleton<ProductType>();
            services.AddSingleton<ProductInputType>();

            var sp = services.BuildServiceProvider();
            services.AddSingleton<ISchema>(new ProductSchema(new FuncDependencyResolver(type => sp.GetService(type))));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "ProductAPI", Version = "v1" });
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
                var dbContext = services.GetService<ProductApiContext>();
                var dbInitializer = services.GetService<IDbInitializer>();
                dbInitializer.Initialize(dbContext);
            }
            
            // Create a message listener in a separate thread.
            Task.Factory.StartNew(() =>
                new MessageListener(app.ApplicationServices, cloudAMQPConnectionString).Start());

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            //app.UseHttpsRedirection();
            app.UseGraphiQl();
            app.UseMvc();
        }
    }
}
