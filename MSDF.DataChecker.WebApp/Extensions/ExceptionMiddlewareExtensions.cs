// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MSDF.DataChecker.Services;
using MSDF.DataChecker.Services.Models;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MSDF.DataChecker.WebApp.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ExceptionMiddleware>();
        }
    }

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, ILogService _logService, ILogger<ExceptionMiddleware> _logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                string information = JsonConvert.SerializeObject(new
                {
                    ex.Message,
                    MessageStack = ex.StackTrace,
                    ex.Source,
                    InnerMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty
                });

                _logger.LogError(information);

                try
                {
                    await _logService.AddAsync(new LogBO
                    {
                        DateCreated = DateTime.UtcNow,
                        Source = "API",
                        Information = information
                    });
                }
                catch { }

                await context.Response.WriteAsync(new { ex.Message }.ToString());
            }            
        }
    }
}
