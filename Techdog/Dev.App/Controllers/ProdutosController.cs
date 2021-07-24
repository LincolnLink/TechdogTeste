using AutoMapper;
using Dev.App.ViewModels;
using Dev.Business.Interfaces;
using Dev.Business.Models;
using DevIO.App.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dev.App.Controllers
{
    
    public class ProdutosController : BaseController
    {
        private readonly IProdutoRepository _produtoRepository;
        private readonly IProdutoService _produtoService;      
        private readonly IMapper _mapper;

        public ProdutosController(
            IProdutoRepository produtoRepository,
            IProdutoService produtoService,           
            IMapper mapper,
            INotificador notificador) : base(notificador)
        {
            _produtoRepository = produtoRepository;
            _produtoService = produtoService;            
            _mapper = mapper;
        }


        [Route("lista-de-produtos")]
        public async Task<IActionResult> Index()
        {
            // Pega todos os produtos e seus fornecedores, converte para um IEnumerable "ProdutoViewModel"
            return View(_mapper.Map<IEnumerable<ProdutoViewModel>>(await _produtoRepository.ObterTodos()));
        }


        // GET: Produtos/Details/5
        [AllowAnonymous]
        [Route("dados-do-produto/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            // Pega o produto que tem o id especifico, exiba a tela apenas se ele existir.
            var produtoViewModel = await obterProdById(id);
            if (produtoViewModel == null)
            {
                return NotFound();
            }
            return View(produtoViewModel);
        }


        // GET: Produtos/Create      
        [Route("novo-produto")]
        public IActionResult Create()
        {   
            return View();
        }

        // POST: Produtos/Create       
        [Route("novo-produto")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProdutoViewModel produtoViewModel)
        {
            
            // Se a ModelState não for valida recarrega a pagina.
            if (!ModelState.IsValid) return View(produtoViewModel);

            //upload do arquivo
            var imgPrefixo = Guid.NewGuid() + "_";
            if (!await UploadArquivo(produtoViewModel.ImagemUpload, imgPrefixo))
            {
                return View(produtoViewModel);
            }

            produtoViewModel.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;

            // Uso repositorio para adicionar o produto convertido.
            await _produtoService.Adicionar(_mapper.Map<Produto>(produtoViewModel));

            if (!OperacaoValida()) return View(produtoViewModel);

            return RedirectToAction(actionName: "Index");
        }


        // GET: Produtos/Edit/5       
        [Route("editar-produto/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            //Obtem o produto que vai ser editado
            var produtoViewModel = await obterProdById(id);

            if (produtoViewModel == null)
            {
                return NotFound();
            }

            return View(produtoViewModel);
        }

        // POST: Produtos/Edit/5
        [Route("editar-produto/{id:guid}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, ProdutoViewModel produtoViewModel)
        {
            //Verifica se o id é o mesmo que tem no objeto
            if (id != produtoViewModel.Id) return NotFound();

            // Buscando os dados originais.
            var produtoAtualizacao = await obterProdById(id);
            produtoViewModel.Imagem = produtoAtualizacao.Imagem;

            // Verificando se está valida a ModelState
            if (!ModelState.IsValid) return View(produtoViewModel);

            // Verifica se tem imagem nova.
            if (produtoViewModel.ImagemUpload != null)
            {
                var imgPrefixo = Guid.NewGuid() + "_";
                if (!await UploadArquivo(produtoViewModel.ImagemUpload, imgPrefixo))
                {
                    return View(produtoViewModel);
                }

                produtoAtualizacao.Imagem = imgPrefixo + produtoViewModel.ImagemUpload.FileName;
            }

            produtoAtualizacao.Nome = produtoViewModel.Nome;
            produtoAtualizacao.Descricao = produtoViewModel.Descricao;
            produtoAtualizacao.Valor = produtoViewModel.Valor;
            produtoAtualizacao.Ativo = produtoViewModel.Ativo;

            // Faz a atualização do produto.
            await _produtoService.Atualizar(_mapper.Map<Produto>(produtoAtualizacao));

            if (!OperacaoValida()) return View(produtoViewModel);

            return RedirectToAction(actionName: "Index");
        }

        // GET: Produtos/Delete/5       
        [Route("excluir-produto/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            // Obtem o produto que vai ser deletado
            var produto = await obterProdById(id);

            if (produto == null)
            {
                return NotFound();
            }

            return View(produto);
        }

        // POST: Produtos/Delete/5
        [Route("excluir-produto/{id:guid}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // Obtem o produto que vai ser deletado
            var produto = await obterProdById(id);

            if (produto == null)
            {
                return NotFound();
            }
            // Deleta o produto usando o repositorio.
            await _produtoService.Remover(id);

            if (!OperacaoValida()) return View(produto);

            TempData["Sucesso"] = "Produto excluido com sucesso!";

            return RedirectToAction(actionName: "Index");
        }

        private async Task<ProdutoViewModel> obterProdById(Guid id)
        {
           return _mapper.Map<ProdutoViewModel>(await _produtoRepository.ObterPorId(id));
        }

        private async Task<bool> UploadArquivo(IFormFile arquivo, string imgPrefixo)
        {
            // Verifica se existe arquivo
            if (arquivo.Length <= 0) return false;

            // Cria um caminho
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/imagens", imgPrefixo + arquivo.FileName);

            // Verifica se o arquivo é repetido.
            if (System.IO.File.Exists(path))
            {
                ModelState.AddModelError(key: string.Empty, errorMessage: "Já existe um arquivo com este nome!");
                return false;
            }

            using (var stream = new FileStream(path, FileMode.Create))
            {
                // Faz a gravação no disco.
                await arquivo.CopyToAsync(stream);
            }

            return true;
        }
    }
}
