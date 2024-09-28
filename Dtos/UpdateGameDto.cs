// these DataAnnotations are used for handling invalid i/p so eg. if stringLength is > 100 then it'll not process
using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class UpdateGameDto(
    [Required][StringLength(100)] string Name,
    int GenreId,
    [Range(1, 100)]decimal Price,
    DateOnly ReleaseDate
);