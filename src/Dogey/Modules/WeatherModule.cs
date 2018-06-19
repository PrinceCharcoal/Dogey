﻿using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [RequireEnabled]
    public class WeatherModule : DogeyModuleBase
    {
        private readonly WeatherService _weather;

        public WeatherModule(WeatherService weather)
        {
            _weather = weather;
        }

        [Command("weather")]
        public async Task WeatherAsync([Remainder]string city)
        {
            var forecast = await _weather.GetForecastAsync(city, WeatherUnit.Imperial);
            if (forecast == null)
            {
                await ReplyAsync($"Could not find a city like `{city}`");
                return;
            }

            var weather = forecast.Weather.First();

            var embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithImageUrl(WeatherService.GetIconUrl(weather.IconId))
                .WithTitle(forecast.Name + "'s Weather")
                .WithDescription(weather.Description)
                .WithFooter(_weather.RequestsRemaining.ToString())
                .WithCurrentTimestamp()
                .AddInlineField("Pressure", forecast.Measurements.Pressure + " hPa")
                .AddInlineField("Humidity", forecast.Measurements.Humidity + "%")
                .AddInlineField("Temp", MathHelper.KelvinToFahrenheit(forecast.Measurements.Temperature) + "f")
                .AddInlineField("Temp Range", MathHelper.KelvinToFahrenheit(forecast.Measurements.TemperatureMin) + "f -> " + MathHelper.KelvinToFahrenheit(forecast.Measurements.TemperatureMax) + 'f');
            await ReplyEmbedAsync(embed);
        }
    }
}