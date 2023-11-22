using simpleApi.Dto;
using simpleApi.Request;

namespace simpleApi.Interface;

public interface IExternalDataService
{
    Task<List<ExternalDTO>> GetDataPlaceholder();
    Task<SimplePostResponseDTO> PostDataPlaceholder(SimplePostRequest request);
}