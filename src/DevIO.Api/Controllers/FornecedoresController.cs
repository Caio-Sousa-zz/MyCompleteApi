using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevIO.Api.Controllers
{
    [Route("api/[controller]")]
    public class FornecedoresController : MainController
    {
        private readonly IFornecedorRepository _fornecedorRepository;

        public FornecedoresController(IFornecedorRepository fornecedorRepository)
        {
            _fornecedorRepository = fornecedorRepository;
        }

        [HttpGet("get-all")]
        public async Task<IEnumerable<FornecedoreViewModel>> ObterTodosAsync()
        {
            var fornecedor = await _fornecedorRepository.ObterTodos();

            return fornecedor.Adapt<IEnumerable<FornecedoreViewModel>>();
        }
    }
}