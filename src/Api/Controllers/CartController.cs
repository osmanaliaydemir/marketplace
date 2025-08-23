using Application.Abstractions;
using Application.DTOs.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/cart")]
[Authorize(Roles = "Customer")]
public sealed class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ILogger<CartController> _logger;

    public CartController(ICartService cartService, ILogger<CartController> logger)
    {
        _cartService = cartService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart([FromQuery] long customerId)
    {
        try
        {
            var cart = await _cartService.GetCartAsync(customerId);
            return Ok(cart);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart for customer {CustomerId}", customerId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<CartDto>> CreateCart([FromBody] CreateCartRequest request)
    {
        try
        {
            var cart = await _cartService.CreateCartAsync(request.CustomerId);
            return CreatedAtAction(nameof(GetCart), new { customerId = request.CustomerId }, cart);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating cart for customer {CustomerId}", request.CustomerId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartItemDto>> AddItem([FromBody] CartAddItemRequest request)
    {
        try
        {
            var customerId = GetCustomerIdFromToken(); // Extract from JWT token
            var cartItem = await _cartService.AddItemAsync(customerId, request);
            return Ok(cartItem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to cart");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPut("items/{itemId}")]
    public async Task<ActionResult<CartItemDto>> UpdateItem(long itemId, [FromBody] CartUpdateItemRequest request)
    {
        try
        {
            var customerId = GetCustomerIdFromToken();
            var cartItem = await _cartService.UpdateItemAsync(customerId, itemId, request);
            return Ok(cartItem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating cart item {ItemId}", itemId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpDelete("items/{itemId}")]
    public async Task<ActionResult> RemoveItem(long itemId)
    {
        try
        {
            var customerId = GetCustomerIdFromToken();
            var success = await _cartService.RemoveItemAsync(customerId, itemId);
            
            if (success)
                return NoContent();
            else
                return NotFound(new { error = "Cart item not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cart item {ItemId}", itemId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPut("items/{itemId}/quantity")]
    public async Task<ActionResult> UpdateQuantity(long itemId, [FromBody] UpdateQuantityRequest request)
    {
        try
        {
            var customerId = GetCustomerIdFromToken();
            var success = await _cartService.UpdateItemQuantityAsync(customerId, itemId, request.Quantity);
            
            if (success)
                return NoContent();
            else
                return BadRequest(new { error = "Invalid quantity or item not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating quantity for cart item {ItemId}", itemId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpDelete("clear")]
    public async Task<ActionResult> ClearCart([FromQuery] long customerId)
    {
        try
        {
            var success = await _cartService.ClearCartAsync(customerId);
            
            if (success)
                return NoContent();
            else
                return NotFound(new { error = "Cart not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cart for customer {CustomerId}", customerId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("stores")]
    public async Task<ActionResult<IEnumerable<CartStoreGroupDto>>> GetCartByStores([FromQuery] long customerId)
    {
        try
        {
            var storeGroups = await _cartService.GetCartByStoresAsync(customerId);
            return Ok(storeGroups);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart by stores for customer {CustomerId}", customerId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("stores/{storeId}")]
    public async Task<ActionResult<CartStoreGroupDto>> GetCartForStore(long storeId)
    {
        try
        {
            var customerId = GetCustomerIdFromToken();
            var storeGroup = await _cartService.GetCartForStoreAsync(customerId, storeId);
            return Ok(storeGroup);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart for store {StoreId}", storeId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("validate")]
    public async Task<ActionResult<CartValidationResult>> ValidateCart([FromQuery] long customerId)
    {
        try
        {
            var validationResult = await _cartService.ValidateCartAsync(customerId);
            return Ok(validationResult);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating cart for customer {CustomerId}", customerId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("stats")]
    public async Task<ActionResult<CartStatsDto>> GetCartStats([FromQuery] long customerId)
    {
        try
        {
            var stats = await _cartService.GetCartStatsAsync(customerId);
            return Ok(stats);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cart stats for customer {CustomerId}", customerId);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    private long GetCustomerIdFromToken()
    {
        // Extract customer ID from JWT token
        // This is a placeholder - implement based on your JWT structure
        var customerIdClaim = User.FindFirst("customer_id")?.Value;
        if (long.TryParse(customerIdClaim, out var id))
            return id;
        
        throw new UnauthorizedAccessException("Invalid customer ID in token");
    }
}

// Request DTOs
public sealed record CreateCartRequest(long CustomerId);
public sealed record UpdateQuantityRequest(int Quantity);
