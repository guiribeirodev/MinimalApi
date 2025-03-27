# Teste Técnico

## Motivações tecnologias escolhidas ⚒️

### Back-end ⚙️

- C#/.Net - Segui uma versão simplificada de uma Minimal Api.
- Banco de dados SQLite - Mantive o mesmo banco por Simplicidade de implementação.
- Entity Framework - Mantive algo relacionado ao projeto em Python, mas desta vez utilizando o EF.


### Front-end 🪞

- React - Escolhido com base no que foi sugerido na descrição do teste.
- Tailwind - Simplicidade e familiaridade.

Mantive o mesmo Front-End

## Como rodar o projeto

Necessário ter a ferramenta Dotnet ef para rodar as Migrations, https://www.nuget.org/packages/dotnet-ef

Estando na pasta raíz, basta executar o comando que irá gerar o Banco de Dados:

```bash
dotnet ef database update --project MinimalApi/
```

Após gerado, executar o projeto MinimalApi:

```bash
dotnet run --project MinimalApi/
```

## Informações adicionais

O projeto irá rodar na porta <http://127.0.0.1:8000>, e na porta <http://127.0.0.1:8000/swagger> terá a documentação da API.

## Comandos adicionais

Para rodar os testes:

```bash
dotnet test
```

Caso deseje executar o FrontEnd separado, entrar na pasta FrontEnd e instalar as dependências:

```bash
npm install
```

E executar o projeto:

```bash
npm run dev
```