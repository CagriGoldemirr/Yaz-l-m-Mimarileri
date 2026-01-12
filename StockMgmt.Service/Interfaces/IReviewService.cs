using StockMgmt.Core.DTOs.Review;

namespace StockMgmt.Service.Interfaces;

public interface IReviewService
{
    Task<ReviewResponseDto> CreateAsync(ReviewCreateDto createDto);
    Task<ReviewResponseDto> UpdateAsync(int id, ReviewUpdateDto updateDto);
    Task<ReviewResponseDto?> GetByIdAsync(int id);
    Task<IEnumerable<ReviewResponseDto>> GetAllAsync();
    Task<IEnumerable<ReviewResponseDto>> GetByProductIdAsync(int productId);
    Task<bool> DeleteAsync(int id);
}
