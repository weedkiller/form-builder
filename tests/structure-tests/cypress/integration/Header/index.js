describe('Button', () => {
    it('Button', () => {
      cy.visit('ui-button')
        .toMatchingDOM('smbc-header')
    });
  });