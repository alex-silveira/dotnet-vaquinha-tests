using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using Vaquinha.Tests.Common.Fixtures;
using Xunit;

namespace Vaquinha.AutomatedUITests
{
	public class DoacaoTests : IDisposable, IClassFixture<DoacaoFixture>, 
                                               IClassFixture<EnderecoFixture>, 
                                               IClassFixture<CartaoCreditoFixture>
	{
		private DriverFactory _driverFactory = new DriverFactory();
		private IWebDriver _driver;

		private readonly DoacaoFixture _doacaoFixture;
		private readonly EnderecoFixture _enderecoFixture;
		private readonly CartaoCreditoFixture _cartaoCreditoFixture;

		public DoacaoTests(DoacaoFixture doacaoFixture, EnderecoFixture enderecoFixture, CartaoCreditoFixture cartaoCreditoFixture)
        {
            _doacaoFixture = doacaoFixture;
            _enderecoFixture = enderecoFixture;
            _cartaoCreditoFixture = cartaoCreditoFixture;
        }
		public void Dispose()
		{
			_driverFactory.Close();
		}

		[Fact]
		public void DoacaoUI_AcessoTelaHome()
		{
			// Arrange
			_driverFactory.NavigateToUrl("https://localhost:44317/");
			_driver = _driverFactory.GetWebDriver();

			// Act
			IWebElement webElement = null;
			webElement = _driver.FindElement(By.Name("vaquinha-titulo"));

			// Assert
			webElement.Displayed.Should().BeTrue(because:"logo exibido");
		}
		[Fact]
		public void DoacaoUI_AcessarPaginaDoacao()
		{
			//Arrange
			var doacao = _doacaoFixture.DoacaoValida();
            doacao.AdicionarEnderecoCobranca(_enderecoFixture.EnderecoValido());
            doacao.AdicionarFormaPagamento(_cartaoCreditoFixture.CartaoCreditoValido());
			_driverFactory.NavigateToUrl("https://localhost:44317/");
			_driver = _driverFactory.GetWebDriver();

			//Act
			IWebElement webElement = null;
			webElement = _driver.FindElement(By.Name("btnDoar"));
			webElement.Click();

			//Assert
			_driver.Url.Should().Contain("/Doacoes/Create");
		}

        [Fact]
        public void DoacaoUI_EfetuarDoacaoComSucesso()
		{
			//Arrange
			var doacao = _doacaoFixture.DoacaoValida();
			doacao.AdicionarEnderecoCobranca(_enderecoFixture.EnderecoValido());
			doacao.AdicionarFormaPagamento(_cartaoCreditoFixture.CartaoCreditoValido());
			_driverFactory.NavigateToUrl("https://localhost:44317/");
			_driver = _driverFactory.GetWebDriver();

			//Act
			IWebElement webElement = null;
			webElement = _driver.FindElement(By.Name("btnDoar"));
			webElement.Click();

			//Assert
			_driver.Url.Should().Contain("/Doacoes/Create");

			//Act
			//Preencher dados para efetuar Doação
			Console.WriteLine("Preenchendo os dados do doador");
			webElement = _driver.FindElement(By.Id("valor"));
			webElement.SendKeys("1500");
			webElement = _driver.FindElement(By.Id("DadosPessoais_Nome"));
			webElement.SendKeys("Alex Dev .Net");
			webElement = _driver.FindElement(By.Id("DadosPessoais_Email"));
			webElement.SendKeys("alexarauj0015@hotmail.com");
			webElement = _driver.FindElement(By.Id("DadosPessoais_MensagemApoio"));
			webElement.SendKeys("Espero que minha ajuda possa contribuir para o objetivo esperado!");
			//Preencher Endereço e Telefone
			Console.WriteLine("Preenchendo os dados Endereço e Telefone");
			webElement = _driver.FindElement(By.Id("EnderecoCobranca_TextoEndereco"));
			webElement.SendKeys("QD 108 ST 09");
			webElement = _driver.FindElement(By.Id("EnderecoCobranca_Numero"));
			webElement.SendKeys("222");
			webElement = _driver.FindElement(By.Id("EnderecoCobranca_Cidade"));
			webElement.SendKeys("Brasília");
			webElement = _driver.FindElement(By.Id("estado"));
			webElement.SendKeys("DF");
			webElement = _driver.FindElement(By.Id("cep"));
			webElement.SendKeys("70713000");
			webElement = _driver.FindElement(By.Id("EnderecoCobranca_Complemento"));
			webElement.SendKeys("Asa Norte");
			webElement = _driver.FindElement(By.Id("telefone"));
			webElement.SendKeys("99888887777");
			//Preencher dados do cartão
			Console.WriteLine("Preenchendo os dados do Cartão de crédito");
			webElement = _driver.FindElement(By.Id("FormaPagamento_NomeTitular"));
			webElement.SendKeys("alex");
			webElement = _driver.FindElement(By.Id("cardNumber"));
			webElement.SendKeys("5396480930048094");
			webElement = _driver.FindElement(By.Id("validade"));
			webElement.SendKeys("111202");
			webElement = _driver.FindElement(By.Id("cvv"));
			webElement.SendKeys("859");
			//Clicar no botão doar para finalizar a operação
			Console.WriteLine("Clicando botão 'Doar', para efetuar a doação");
			webElement = _driver.FindElement(By.XPath("//input[contains(@value, 'Doar')]"));
			webElement.Click();

			//Assert
			_driver.Url.Should().Contain("/");
			Assert.NotNull(webElement = _driver.FindElement(By.XPath("/html/body/div/main/div/h4[1]")));
			webElement.Should().Equals("Quem ja doou?");
			Assert.NotNull(webElement = _driver.FindElement(By.XPath("//h1[contains(@class, 'display-5 text-center')]")));
			webElement.Should().Equals("alex - R$ 1.500,00");

		}
	}
}