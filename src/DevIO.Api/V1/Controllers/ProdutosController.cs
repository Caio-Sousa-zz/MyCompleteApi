using DevIO.Api.Controllers;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DevIO.Api.V1.Controllers
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProdutosController : MainController
    {
        private IProdutoRepository _produtoRepository { get; set; }
        private IProdutoService _produtoService { get; set; }

        public ProdutosController(INotificador notificador,
                                  IProdutoRepository produtoRepository,
                                  IProdutoService produtoService,
                                  IUser user) : base(notificador, user)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;
        }

        [HttpGet]
        public async Task<IEnumerable<ProdutoViewModel>> ObterTodos()
        {
            TypeAdapterConfig<Produto, ProdutoViewModel>.NewConfig().Map(b => b.NomeFornecedor, a => a.Fornecedor.Nome);

            var produtos = await _produtoRepository.ObterProdutosFornecedores();

            return produtos.Adapt<IEnumerable<ProdutoViewModel>>();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> ObterPorId(Guid id)
        {
            var produtoViewModel = await ObterPorId(id);

            if (produtoViewModel == null) return NotFound();

            return produtoViewModel;
        }

        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<ProdutoViewModel>> Excluir(Guid id)
        {
            var produtoViewModel = await ObterPorId(id);

            if (produtoViewModel == null) return NotFound();

            await _produtoRepository.Remover(id);

            return CustomResponse(produtoViewModel);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoViewModel>> Adicionar(ProdutoViewModel produtoViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imagemNome = $"{Guid.NewGuid()}_{produtoViewModel.Imagem}";

            if (!UploadArquivo(produtoViewModel.ImagemUpload, imagemNome))
            {
                return CustomResponse(produtoViewModel);
            }

            produtoViewModel.Imagem = imagemNome;

            await _produtoService.Adicionar(produtoViewModel.Adapt<Produto>());

            return CustomResponse(produtoViewModel);
        }

        [HttpPost("Adicionar")]
        public async Task<ActionResult<ProdutoImagemViewModel>> AdicionarAlternativo(IFormFile imagem, ProdutoImagemViewModel produtoImagemViewModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgPrefix = $"{Guid.NewGuid()}_";

            if (!await UploadArquivoAlternativo(produtoImagemViewModel.ImagemUpload, imgPrefix))
            {
                return CustomResponse(produtoImagemViewModel);
            }

            produtoImagemViewModel.Imagem = imgPrefix + produtoImagemViewModel.ImagemUpload.FileName;

            await _produtoRepository.Adicionar(produtoImagemViewModel.Adapt<Produto>());

            return CustomResponse(produtoImagemViewModel);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<ProdutoImagemViewModel>> Atualizar(Guid id, ProdutoViewModel produtoViewModel)
        {
            if (id != produtoViewModel.Id)
            {
                NotificarErro("O id está invaálido");
                return CustomResponse(produtoViewModel);
            }

            var produtoAtualizacao = await ObterProduto(id);
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if (produtoViewModel.ImagemUpload != null)
            {
                var nomeImagem = $"{Guid.NewGuid()}_{produtoViewModel.Imagem}";

                if (!UploadArquivo(produtoViewModel.ImagemUpload, nomeImagem))
                    return CustomResponse(ModelState);

                produtoAtualizacao.Imagem = nomeImagem;
            }

            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;

            await _produtoRepository.Atualizar(produtoAtualizacao.Adapt<Produto>());

            return CustomResponse(produtoViewModel);
        }

        [DisableRequestSizeLimit()]
        [HttpPost("imagem")]
        public async Task<ActionResult> AdicionarImagem(IFormFile imagem)
        {
            return Ok(imagem);
        }

        private bool UploadArquivo(string arquivo, string nomeImagem)
        {
            var imageDataByteArray = Convert.FromBase64String(arquivo);

            if (string.IsNullOrEmpty(arquivo))
            {
                NotificarErro("Forneça uma imagem para este produto!");
                return false;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", nomeImagem);

            if (System.IO.File.Exists(filePath))
            {
                NotificarErro("Forneça uma imagem para este produto");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

            return true;
        }

        private async Task<bool> UploadArquivoAlternativo(IFormFile arquivo, string imgPrefixo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                NotificarErro("Forneça uma imagem para este produto!");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + arquivo.FileName);

            if (System.IO.File.Exists(path))
            {
                NotificarErro("Já existe um arquivo com este nome!");
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return true;
        }

        private async Task<ProdutoViewModel> ObterProduto(Guid id)
        {
            return _produtoRepository.ObterPorId(id).Adapt<ProdutoViewModel>();
        }
    }
}