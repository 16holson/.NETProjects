using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using Hangman.Shared;

namespace Hangman.Server.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class AssetController : ControllerBase {

        private string? randomWord = "testestest";

        [HttpGet("Word")]
        public ActionResult Get() {

            //string path = @"assets\random_words.txt";


            //var str = File.ReadAllText(path);

            //Random rand = new Random();
            //int randomNum = rand.Next(0, 10001);
            //var splitwords = str.Split("\n");
            //randomWord = splitwords[randomNum].Replace("\n", "");


            var str = File("random_words.txt", "plain/text");
            randomWord = str.ToString();



            return Ok(randomWord);
        }


    }
}
