using System.ComponentModel.DataAnnotations;
namespace GameStore.Api.Dtos;

public record class GenreDto(
    
    int Id, 

    [StringLength(100)]string Name
);