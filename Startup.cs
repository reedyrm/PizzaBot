// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.3.0

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using PizzaBot.Bots;

namespace PizzaBot
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

            // Create the credential provider to be used with the Bot Framework Adapter.
            services.AddSingleton<ICredentialProvider, ConfigurationCredentialProvider>();

            // Create the Bot Framework Adapter.
            services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();

            // Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.) 
            // Added Local Memory Storage for the user to simulate a DB or longer running store
            services.AddSingleton<IStorage, MemoryStorage>();

            // Create the User state. 
            // Added to store UserState which is about the user and longer lived
            services.AddSingleton<UserState>();

            // Create the Conversation state.
            // Added ConversationState to store local data for immediate conversation
            services.AddSingleton<ConversationState>();

            services.AddSingleton<PizzaDialog>();

            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            //services.AddTransient<IBot, EchoBot>();
            services.AddTransient<IBot, PizzaBot<PizzaDialog>>();
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

            app.UseDefaultFiles();
            app.UseStaticFiles();

            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
