using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopAPI.DTOs;
using ShopAPI.Services.Customer.IServices;
using ShopDAL.Models;
using System.Security.Claims;

namespace ShopAPI.Controllers.Customer
{
    [Route("api/customer/address")]
    [ApiController]
    [Authorize]
    public class CustomerAddressController : ControllerBase
    {
        private readonly ICustomerAddressService _customerAddressService;

        public CustomerAddressController(ICustomerAddressService customerAddressService)
        {
            _customerAddressService = customerAddressService;
        }

        [HttpGet]
        public IActionResult GetAddress()
        {
            try
            {
                var userId = GetCurrentUserId();
                var addresses = _customerAddressService
                    .GetByUserId(userId)
                    .Select(ToDto)
                    .ToList();

                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("default")]
        public IActionResult GetDefaultAddress()
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = _customerAddressService.GetAddressDefault(userId);

                if (address == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Default address not found"
                    });
                }

                return Ok(ToDto(address));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = _customerAddressService.GetById(id);

                if (address == null || address.UserId != userId)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Address not found"
                    });
                }

                return Ok(ToDto(address));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateAddressDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                _customerAddressService.Add(userId, dto);

                return Ok(new
                {
                    success = true,
                    message = "Address created"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateAddressDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (id != dto.AddressId)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Address ID mismatch"
                    });
                }

                _customerAddressService.Update(userId, dto);

                return Ok(new
                {
                    success = true,
                    message = "Address updated"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var address = _customerAddressService.GetById(id);

                if (address == null || address.UserId != userId)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Address not found"
                    });
                }

                _customerAddressService.Delete(id);

                return Ok(new
                {
                    success = true,
                    message = "Address deleted"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}/default")]
        public IActionResult SetDefault(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                _customerAddressService.SetDefault(userId, id);

                return Ok(new
                {
                    success = true,
                    message = "Default address updated"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }

            return userId;
        }

        private static AddressDto ToDto(Address address)
        {
            return new AddressDto
            {
                AddressId = address.AddressId,
                ReceiverName = address.ReceiverName,
                PhoneNumber = address.PhoneNumber,
                Province = address.Province,
                District = address.District,
                Ward = address.Ward,
                DetailAddress = address.DetailAddress,
                IsDefault = address.IsDefault
            };
        }
    }
}
