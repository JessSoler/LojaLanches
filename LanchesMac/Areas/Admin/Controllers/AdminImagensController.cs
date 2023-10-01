using LanchesMac.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LanchesMac.Areas.Admin.Controllers
{
    //especifica que o controlador pertence à área "Admin". 
    [Area("Admin")]
    //indica que apenas usuários com a função "Admin" têm permissão para acessar
    [Authorize(Roles = "Admin")]
    public class AdminImagensController : Controller
    {
        private readonly ConfigurationImagens _myConfig;

        //fornece informações sobre o ambiente de hospedagem, como caminhos de arquivos
        private readonly IWebHostEnvironment _hostingEnvironment;
        //fornece configurações relacionadas a imagens a partir de um provedor de configurações.
        public AdminImagensController(IWebHostEnvironment hostingEnvironment, 
            IOptions<ConfigurationImagens> myConfiguration)
        {
            _hostingEnvironment = hostingEnvironment;
            _myConfig = myConfiguration.Value;
        }

        public IActionResult Index()
        {
            return View();
        }
        //Esta ação lida com o envio de arquivos(imagens) para o servidor.
        //Verifique se os arquivos foram enviados e se a quantidade de arquivos não excede o limite.
        //Calcular o tamanho total dos arquivos.
        //Salve os arquivos no servidor e registre o caminho de cada arquivo.
        //Retorna informações sobre o resultado do envio na ViewData
        public async Task<IActionResult> UploadFiles(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                ViewData["Erro"] = "Error: Arquivo(s) não selecionado(s)";
                return View(ViewData);
            }

            if (files.Count > 10)
            {
                ViewData["Erro"] = "Error: Quantidade de arquivos excedeu o limite";
                return View(ViewData);
            }

            long size = files.Sum(f => f.Length);
            var filePathsName = new List<string>();
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath,
                   _myConfig.NomePastaImagensProdutos);

            foreach (var formFile in files)
            {
               if (formFile.FileName.Contains(".jpg") || formFile.FileName.Contains(".gif") ||
                        formFile.FileName.Contains(".png"))
                {
                   var fileNameWithPath = string.Concat(filePath, "\\", formFile.FileName);
                   
                   filePathsName.Add(fileNameWithPath);

                   using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                   {
                     await formFile.CopyToAsync(stream);
                   }
               }
            }
            //monta a ViewData que será exibida na view como resultado do envio 
            ViewData["Resultado"] = $"{files.Count} arquivos foram enviados ao servidor, " +
             $"com tamanho total de : {size} bytes";

            ViewBag.Arquivos = filePathsName;

            //retorna a viewdata
            return View(ViewData);
        }
        
        //obtém uma lista de imagens existentes no servidor e é exibida em uma visualização.
        public IActionResult GetImagens()
        {
            FileManagerModel model = new FileManagerModel();

            var userImagesPath = Path.Combine(_hostingEnvironment.WebRootPath,
                 _myConfig.NomePastaImagensProdutos);

            DirectoryInfo dir = new DirectoryInfo(userImagesPath);
            FileInfo[] files = dir.GetFiles();
            model.PathImagesProduto = _myConfig.NomePastaImagensProdutos;

            if (files.Length == 0)
            {
                ViewData["Erro"] = $"Nenhum arquivo encontrado na pasta {userImagesPath}";
            }

            model.Files = files;
            return View(model);
        }

        //lida com a exclusão de um arquivo de imagem no servidor com base em seu nome de arquivo.
        public IActionResult Deletefile(string fname)
        {
            string _imagemDeleta = Path.Combine(_hostingEnvironment.WebRootPath,
                _myConfig.NomePastaImagensProdutos + "\\", fname);

            if ((System.IO.File.Exists(_imagemDeleta)))
            {
                System.IO.File.Delete(_imagemDeleta);
                ViewData["Deletado"] = $"Arquivo(s) {_imagemDeleta} deletado com sucesso";
            }
            return View("index");
        }
    }
}
