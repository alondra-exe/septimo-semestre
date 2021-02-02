using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PokedexWEB.Models;

namespace PokedexWEB.Controllers
{
    public class HomeController : Controller
    {
        public IHttpClientFactory Factory { get; set; }
        HttpClient client;

        public HomeController(IHttpClientFactory clientFactory)
        {
            Factory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            List<DatosPokemon> pokemons = new List<DatosPokemon>();
            try
            {
                for (int pokeid = 0; pokeid < 152; pokeid++)
                {
                    client = Factory.CreateClient("pokedex");
                    var response = await client.GetAsync($"https://pokeapi.co/api/v2/pokemon/{pokeid}");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        pokemons.Add(JsonConvert.DeserializeObject<DatosPokemon>(json));
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return View(pokemons);
        }
    }
}