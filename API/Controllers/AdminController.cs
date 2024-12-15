using API.DTOs;
using API.Extensions;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController(IUnitOfWork unit, IPaymentService paymentService) : BaseApiController
    {
        [HttpGet("orders")]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders([FromQuery] OrderSpecParams specParams)
        {
            var spec = new OrderSpecification(specParams);
            return await CreatePagedResult(unit.Repository<Order>(), spec, specParams.PageIndex, specParams.PageSize, o => o.ToDto());
        }

        [HttpGet("orders/{id:int}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            var spec = new OrderSpecification(id);
            var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null)
                return BadRequest("Ne postoji narudžba sa tim id-em.");
            return order.ToDto();

        }
        [HttpPost("orders/refund/{id:int}")]
        public async Task<ActionResult<OrderDto>> RefundOrder(int id)
        {
            var spec = new OrderSpecification(id);

            var order = await unit.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null) return BadRequest("Ne postoji navedena narudzba.");

            if (order.Status == OrderStatus.Pending)
                return BadRequest("Uplata nije uspjela za ovu porudžbinu.");
            var result = await paymentService.RefundPayment(order.PaymentIntentId);
            if (result == "succeeded")
            {
                order.Status = OrderStatus.Refunded;
                await unit.Complete();
                return order.ToDto();
            }
            return BadRequest("Problem refunding order.");

            
        }
    }
}
