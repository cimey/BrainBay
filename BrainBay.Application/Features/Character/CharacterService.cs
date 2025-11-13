using BrainBay.Core.Repositories;
using BrainBay.Model.Inputs;
using BrainBay.Model.Requests;
using BrainBay.Model.Responses;
using CharacterEntity = BrainBay.Core.Entities.Character;
using BrainBay.Core.ValueTypes;
using AutoMapper;

namespace BrainBay.Application.Features.Character
{

    public interface ICharacterService
    {
        Task<IEnumerable<CharacterDto>> GetCharactersAsync(GetCharactersRequest input);

        Task<CharacterDto> GetCharacterAsync(int id);

        Task<CharacterDto> CreateCharacterAsync(CreateCharacterInput input);

        Task InvalidateCache();
    }

    public class CharacterService : ICharacterService
    {
        private readonly ICharacterRepositoryDecorator _cachedCharacterRepository;

        private readonly IMapper _mapper;
        public CharacterService(ICharacterRepositoryDecorator characterRepository, IMapper mapper)
        {
            _cachedCharacterRepository = characterRepository;
            _mapper = mapper;
        }

        public async Task<CharacterDto> CreateCharacterAsync(CreateCharacterInput input)
        {
            var entity = await _cachedCharacterRepository.AddAsync(new CharacterEntity(input.Name, input.Status, input.Species, input.Type, input.Gender,
           input.Origin != null ? new Origin(input.Origin.Name, input.Origin.Url) : null,
           input.Location != null ? new Location(input.Location.Name, input.Location.Url) : null,
           input.Image, string.Join(",", input.Episode)));
            
            
            await InvalidateCache(); //TODO: Just add new entity to Redis

            return _mapper.Map<CharacterDto>(entity);
        }

        public async Task<CharacterDto?> GetCharacterAsync(int id)
        {
            return _mapper.Map<CharacterDto?>(await _cachedCharacterRepository.GetByIdAsync(id));
        }

        public async Task<IEnumerable<CharacterDto>> GetCharactersAsync(GetCharactersRequest input)
        {
            var result = (await _cachedCharacterRepository.GetQueryableAsync()).Skip(input.Skip).Take(input.PageSize).ToList();

            return _mapper.Map<IEnumerable<CharacterDto>>(result);
        }

        public async Task InvalidateCache()
        {
            await _cachedCharacterRepository.InvalidateCache();
        }
    }
}
