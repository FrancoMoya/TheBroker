using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using TheBroker.DTOs;
using TheBroker.Models;

namespace TheBroker.Controllers

{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BrokerController : ControllerBase
    {
        private readonly DbOrdersContext _context;

        public BrokerController(DbOrdersContext context)
        {
            _context = context;
        }

        //Listar ordenes
        [HttpGet("Orders/ListOrders")]
        public async Task<ActionResult<List<ListOrdersDTO>>> GetOrders()
        {
            var orders = await _context.Set<ListOrdersDTO>().FromSqlRaw("SP_ORDER_LIST").ToListAsync();
            return Ok(orders);
        }
        //Listar ordenes por ID
        [HttpGet("Orders/GetOrder/{id}")]
        public async Task<ActionResult<List<ListOrdersDTO>>> GetOrderById(int id)
        {
            var order = await _context.Set<ListOrdersDTO>().FromSqlInterpolated($"SP_ORDER_LIST_TX {id}").ToListAsync();
            if (order.Count == 0)
            {
                return NotFound($"No existe una orden con el TX_NUMBER: {id}");
            }
            return Ok(order);
        }
        //Crear ordenes
        [HttpPost("Orders/NewOrder")]
        public async Task<ActionResult<OrderDetailsDTO>> CreateOrder([FromBody] CreateOrdersDTO request)
        {
            var result = await _context.Set<OrderDetailsDTO>()
                .FromSqlInterpolated($"EXEC SP_ORDER_CREATE {request.ACTION}, {request.STATUS}, {request.ID_SYMBOL}, {request.QUANTITY}")
                .ToListAsync();
            return Ok(result.First());
        }
        //Eliminar Orden
        [HttpDelete("Orders/DeleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrderById(int id)
        {
            var orderExists = await _context.Set<ListOrdersDTO>().FromSqlInterpolated($"SP_ORDER_LIST_TX {id}").ToListAsync();

            if (orderExists.Count == 0)
            {
                return NotFound($"No se encontró una orden con el ID: {id}");
            }

            await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC SP_ORDER_DELETE {id}");

            return Ok($"La orden con ID: {id} ha sido eliminada correctamente.");
        }
        // Modificar estado de una orden
        [HttpPatch("Orders/ChangeStatus/{id}")]
        public async Task<ActionResult<ListOrdersDTO>> ChangeStatus(int id, [FromBody] ChangeStatusDTO request)
        {
            var result = await _context.Set<OrderDetailsDTO>()
                .FromSqlInterpolated($"EXEC SP_ORDERS_CHANGED {id}, {request.STATUS}")
                .ToListAsync();
            return Ok(result.First());
        }
        //Listar ordenes EXECUTED
        [HttpGet("Orders/ListOrdersExecuted")]
        public async Task<ActionResult<List<ListOrdersExecutedDTO>>> GetOrdersExecuted()
        {
            var orders = await _context.Set<ListOrdersExecutedDTO>().FromSqlRaw("SP_LIST_ORDERS_EXECUTED").ToListAsync();
            return Ok(orders);
        }
        //Listar ordenes por AÑO
        [HttpGet("Orders/GetOrderByYear/{year}")]
        public async Task<ActionResult<List<ListOrdersDTO>>> GetOrderByYear(int year)
        {
            var order = await _context.Set<ListOrdersDTO>().FromSqlInterpolated($"SP_LIST_ORDERS_BY_YEAR {year}").ToListAsync();
            if (order.Count == 0)
            {
                return NotFound($"No existe una orden con el año: {year}");
            }
            return Ok(order);
        }
        //Crear orden con estado EXECUTED
        [HttpPost("Orders/NewOrderExecuted")]
        public async Task<ActionResult<OrderDetailsDTO>> CreateOrderExecuted([FromBody] CreateOrderExecutedDTO request)
        {
            var result = await _context.Set<OrderDetailsDTO>()
                .FromSqlInterpolated($"EXEC SP_ORDER_BUY_OR_SELL_PENDING {request.ACTION}, {request.ID_SYMBOL}, {request.QUANTITY}")
                .ToListAsync();
            return Ok(result.First());
        }


        //Listar stock
        [HttpGet("StockMS/ListStock")]
        public async Task<ActionResult<List<ListStockDTO>>> GetStock()
        {
            var stock = await _context.Set<ListStockDTO>().FromSqlRaw("SP_STOCK_LIST").ToListAsync();
            return Ok(stock);
        }
        //Crear stock
        [HttpPost("StockMS/NewStock")]
        public async Task<ActionResult<StockDetailsDTO>> CreateStock([FromBody] CreateStockDTO request)
        {
            var result = await _context.Set<StockDetailsDTO>()
                .FromSqlInterpolated($"EXEC SP_STOCK_CREATE {request.SYMBOL}, {request.UNIT_PRICE}")
                .ToListAsync();
            return Ok(result.First());

        }
        //Listar stock por ID
        [HttpGet("StockMS/GetStock/{id}")]
        public async Task<ActionResult<List<ListStockDTO>>> GetStockById(int id)
        {
            var order = await _context.Set<ListStockDTO>().FromSqlInterpolated($"SP_STOCK_LIST_PK {id}").ToListAsync();
            if (order.Count == 0)
            {
                return NotFound($"No existe un stock con el ID: {id}");
            }
            return Ok(order);
        }
        //Eliminar Stock
        [HttpDelete("StockMS/DeleteStock/{id}")]
        public async Task<IActionResult> DeleteStockById(int id)
        {
            // Verificar si el stock existe antes de intentar eliminarlo
            var stockExists = await _context.Set<ListStockDTO>().FromSqlInterpolated($"SP_STOCK_LIST_PK {id}").ToListAsync();

            if (stockExists.Count==0)
            {
                return NotFound($"No se encontró un stock con el ID: {id}");
            }

            // Si el stock existe, proceder a eliminarlo
            await _context.Database.ExecuteSqlInterpolatedAsync($"EXEC SP_STOCK_DELETE {id}");

            return Ok($"El stock con ID: {id} ha sido eliminado correctamente.");
        }


        //Listar orders history
        [HttpGet("OrdersHistory/ListOrdersHistory")]
        public async Task<ActionResult<List<ListOhDTO>>> GetOrdersHistory()
        {
            var result = await _context.Set<ListOhDTO>().FromSqlRaw("SP_OH_LIST").ToListAsync();
            return Ok(result);
        }
    }
}
