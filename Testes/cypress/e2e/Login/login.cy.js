describe("Login na aplicação Yrke", () => {

  // Sempre acessa a página de login antes de cada teste
  beforeEach(() => {
    cy.visit("/Account/Login");
  });

  it("Login com sucesso", () => {
    cy.get('input[name="Email"]').type("barbosavanderson@hotmail.com", { delay: 100 });
    cy.get('input[name="Senha"]').type("123456", { delay: 100 });
    cy.get('button[type="submit"]').click();

        //Precisa sair da página de login para confirmar o sucesso do login
    cy.url().should("not.include", "/Account/Login");

    // testando se o modal é aparece
    cy.get('#welcomeModal').should("be.visible");
    cy.get('#welcomeModal').contains("Bem-vindo ao Yrke").should("be.visible");

    // Fechando o modal
    cy.get('button[data-dismiss="modal"]').click();
    cy.get('welcomeModal').should("not.exist");

   
  });

  it("Não deve logar com senha incorreta", () => {
  cy.get('input[name="Email"]').type("barbosavanderson@hotmail.com", { delay: 100 });
  cy.get('input[name="Senha"]').type("123123", { delay: 100 });
  cy.get('button[type="submit"]').click();

  // Espera o modal aparecer
  cy.get('#errorModal', { timeout: 10000 }).should("be.visible");

  // Verifica a mensagem
  cy.get('#errorModal').contains("Gentileza verificar email e senha").should("be.visible");

  // Fecha o modal usando o seletor correto
  cy.get('#errorModal .modal-footer > .btn').click({ force: true });
});



});
