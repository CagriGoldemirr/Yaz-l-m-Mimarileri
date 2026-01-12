using StockMgmt.Core.DTOs.User;

namespace StockMgmt.Service.Interfaces;

public interface IUserService
{
    Task<UserResponseDto> CreateAsync(UserCreateDto createDto);
    Task<UserResponseDto> UpdateAsync(int id, UserUpdateDto updateDto);
    Task<UserResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<UserResponseDto>> GetAllAsync();
    Task<bool> DeleteAsync(int id);
}

