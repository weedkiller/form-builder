describe('PageContent', () => {
    it('PageContent', () => {
      cy.visit('ui-page-content')
        .toMatchingDOM()
    });
  });