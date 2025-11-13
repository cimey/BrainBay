using BrainBay.Application.Features.Character;
using BrainBay.Model.Inputs;
using BrainBay.Model.Requests;
using Microsoft.AspNetCore.Mvc;

namespace BrainBay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharactersController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharactersController(ICharacterService characterService)
        {
            _characterService = characterService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCharacters([FromQuery] int skip, [FromQuery] int take)
        {
            var characters = await _characterService.GetCharactersAsync(new GetCharactersRequest { Skip = skip, PageSize = take });
            return Ok(characters);
        }
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var character = await _characterService.GetCharacterAsync(id);
            if (character == null)
                return NotFound();
            return Ok(character);
        }


        [HttpPost]
        public async Task<IActionResult> CreateCharacter([FromBody] CreateCharacterInput input)
        {
            var character = await _characterService.CreateCharacterAsync(input);
            return CreatedAtAction(nameof(GetCharacters), new { id = character.Id }, character);
        }

        [HttpPost]
        [Route("invalidate-cache")]
        public async Task<IActionResult> InvalidateCache()
        {
            await _characterService.InvalidateCache();
            return Ok();
        }
    }
}
