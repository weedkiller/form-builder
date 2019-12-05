﻿@addressManual
Feature: AddressManual
	In order to collect address information I have to navigate to Page1


Scenario: Renders HTML tags on the page
	Given I navigate to "/address/page1"
	Then I should see the header
	And I should see the "customers-address-postcode" input
	And I should see the "manual" link
	And I should see the "nextStep" button


Scenario: Renders HTML tags on the manual page
	Given I navigate to "/address/page1"
	Then  I click the manual link
	Then I should see the "customers-address-AddressManualAddressLine1" input
	Then I should see the "customers-address-AddressManualAddressLine2" input
	Then I should see the "customers-address-AddressManualAddressTown" input
	Then I should see the "customers-address-AddressManualAddressPostcode" input


Scenario: Shows Error Messages on the manual page when I dont enter amything
	Given I navigate to "/address/page1"
	Then  I click the manual link
	When I click the "nextStep" button
	Then I should see a "p" element with "Please enter Address Line 1" text
