﻿@successpage
Feature: SuccessPage
	In order to submit a form
	as a User
	I should see a success page

Scenario: Success page standard use
	Given I navigate to "/checkbox/page1"
	When I click the "CheckBoxList-0" checkbox
	Then I click the "CheckBoxList-1" checkbox
	Then I click the "nextStep" button
	Then I click the "submit" button
	Then I should see a ".smbc-panel" html element
	Then I should see a ".govuk-panel__title" html element
	Then I should see a ".govuk-panel__body" html element
	Then I should see a "p" element with "The wikipedia page on fruit is at Fruits" text