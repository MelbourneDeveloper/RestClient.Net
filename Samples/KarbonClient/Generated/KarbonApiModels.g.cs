namespace Karbon.Generated;

/// <summary>A Karbon invoice</summary>
/// <param name="InvoiceKey">The unique key assigned to an invoice by Karbon</param>
/// <param name="InvoiceNumber">The invoice number of the invoice</param>
/// <param name="TotalAmountDue">Remaining amount due on the invoice, including taxes minus any payments</param>
/// <param name="InvoiceTotal">Invoice total, including taxes</param>
/// <param name="InvoiceSubTotal">The sum of the line items of the invoice, excluding taxes</param>
/// <param name="InvoiceTaxTotal">Total of the taxes on the invoice</param>
/// <param name="InvoiceDate">The date the invoice was issued as an ISO 8601 timestamp</param>
/// <param name="PaymentDueDate">The date the invoice is due as an ISO 8601 timestamp</param>
/// <param name="UpdatedAt">A timestamp showing when the invoice was last updated as an ISO 8601 timestamp</param>
/// <param name="CurrencyCode">A three letter ISO 4217 currency code denoting the currency used in the invoice, e.g. AUD, USD, CAD, NZD, EUR, GBP</param>
/// <param name="PaymentInstructions">Payment instructions added to the invoice</param>
/// <param name="InvoiceStatus">Current status of the invoice</param>
public record BillingBaseInvoice(string InvoiceKey, string InvoiceNumber, float TotalAmountDue, float InvoiceTotal, float InvoiceSubTotal, float InvoiceTaxTotal, string InvoiceDate, string PaymentDueDate, string UpdatedAt, string CurrencyCode, object PaymentInstructions, string InvoiceStatus);

/// <summary>A Karbon Invoice record</summary>

public record BillingInvoice();

/// <summary>A Karbon invoice with associated payment records</summary>

public record BillingInvoiceWithPayment();

/// <summary>A collection of the data associated with an invoice</summary>
/// <param name="Data">The collection of data that is used to create the rendered (HTML/PDF) invoice sent to the billing contact</param>
public record BillingInvoiceData(List<BillingInvoiceDataItem> Data);

/// <summary>A single component of the data used to create the rendered (HTML/PDF) invoice</summary>
/// <param name="EntityKey">The unique key of the source of the billing</param>
/// <param name="EntityType">The type of the source of the billing</param>
/// <param name="Billed">The value of the billed amount</param>
/// <param name="Expenses">Expenses</param>
/// <param name="Service">Service</param>
/// <param name="Work">Work</param>
public record BillingInvoiceDataItem(string EntityKey, string EntityType, double Billed, BillingInvoiceDataExpense Expenses, BillingInvoiceDataService Service, BillingInvoiceDataWork Work);

/// <summary>A breakdown of tax related to invoice, lineitems and data</summary>
/// <param name="TaxRate">The tax rated applied to the billed amount</param>
/// <param name="TaxRateName">The name of the tax rate applied to the billed amount</param>
/// <param name="TaxAmount">The value of tax component in the billed amount</param>
public record BillingInvoiceDataTax(object TaxRate, object TaxRateName, object TaxAmount);

/// <summary>A line item on a Karbon invoice</summary>
/// <param name="Billed">The total billed value of expenses</param>
/// <param name="Expenses">Expenses</param>
public record BillingInvoiceDataExpense(float Billed, List<BillingInvoiceDataExpenseItem> Expenses);

/// <summary>An expense line item on a Karbon invoice</summary>

public record BillingInvoiceDataExpenseItem();

/// <summary>A service line item on a Karbon invoice</summary>

public record BillingInvoiceDataService();

/// <summary>The breakdown of the billing that stems from work</summary>

public record BillingInvoiceDataWork();

/// <summary>The details of an invoiced time entry</summary>
/// <param name="TimeEntryKey">The unique identifier of the time entry</param>
/// <param name="ColleagueKey">The unique ID of the user that logged the time</param>
/// <param name="RoleName">The role on the time entry</param>
/// <param name="TaskTypeName">The task ype on the time entry</param>
/// <param name="Billed">The billed value of the time entry</param>
public record BillingInvoiceDataWorkTimeEntry(string TimeEntryKey, string ColleagueKey, string RoleName, string TaskTypeName, float Billed);

/// <summary>A Karbon Payment record</summary>

public record BillingPayment();

/// <summary>A breakdown of line items from the invoice</summary>
/// <param name="LineItems">An array of invoice line items</param>
public record BillingInvoiceLineItems(List<object> LineItems);

/// <summary>Details of the payments related to the invoice</summary>
/// <param name="Payments">An array of invoice payments</param>
public record BillingInvoicePayments(List<object> Payments);

/// <summary>The details of the invoice recipient at the point in time the invoice was drafted or payment creator at the point the payment was received</summary>
/// <param name="Client">The Karbon Client Billing Contact</param>
public record BillingClient(object Client);

/// <summary>Details of the tax components of the invoice</summary>
/// <param name="TaxLineItems">An array of tax types and values from invoice line items</param>
public record BillingTaxLineItems(List<object> TaxLineItems);

/// <summary>A business card associated with a contact or organization</summary>
/// <param name="IsPrimaryCard">Set to true, if this business card is also the primary business card for this entity.</param>
/// <param name="OrganizationKey">Used on the Business Card for a Contact to specify the Organization that the Contact is related to. Will always be null for an Organization business card.</param>
/// <param name="WebSites">The websites associated with this Business card.</param>
/// <param name="EmailAddresses">The email addresses associated with this Business card.</param>
/// <param name="RoleOrTitle">The business role or title of the contact this Business Cards belongs to</param>
/// <param name="FacebookLink">The URL to the entity's Facebook profile</param>
/// <param name="LinkedInLink">The URL to the entity's LinkedIn profile</param>
/// <param name="TwitterLink">The URL to the entity's Twitter profile</param>
/// <param name="SkypeLink">The URL to the entity's Skype profile</param>
/// <param name="Addresses">Addresses</param>
/// <param name="PhoneNumbers">PhoneNumbers</param>
public record BusinessCardRequest(bool IsPrimaryCard, object OrganizationKey, List<string> WebSites, List<string> EmailAddresses, object RoleOrTitle, string FacebookLink, string LinkedInLink, string TwitterLink, string SkypeLink, List<object> Addresses, List<object> PhoneNumbers);

/// <summary>A business card associated with a contact or organization</summary>
/// <param name="BusinessCardKey">A Karbon-generated unique identifier for the Business Card</param>
/// <param name="IsPrimaryCard">Set to true, if this business card is also the primary business card for this entity.</param>
/// <param name="WebSites">The websites associated with this Business card.</param>
/// <param name="EmailAddresses">The email addresses associated with this Business card.</param>
/// <param name="OrganizationKey">A Karbon-generated key to indicate the organization this Business Card belongs to</param>
/// <param name="RoleOrTitle">The business role or title of the contact this Business Cards belongs to</param>
/// <param name="FacebookLink">The URL to the entity's Facebook profile</param>
/// <param name="LinkedInLink">The URL to the entity's LinkedIn profile</param>
/// <param name="TwitterLink">The URL to the entity's Twitter profile</param>
/// <param name="SkypeLink">The URL to the entity's Skype profile</param>
/// <param name="Addresses">Addresses</param>
/// <param name="PhoneNumbers">PhoneNumbers</param>
public record BusinessCard(string BusinessCardKey, bool IsPrimaryCard, List<string> WebSites, List<string> EmailAddresses, string OrganizationKey, string RoleOrTitle, string FacebookLink, string LinkedInLink, string TwitterLink, string SkypeLink, List<object> Addresses, List<object> PhoneNumbers);

/// <summary>A collection of business cards</summary>

public record BusinessCardsRequestAsArray();

/// <summary>The Client Team associated with a contact or organization</summary>

public record ClientTeam();

/// <summary>A list of the available custom fields in Karbon</summary>
/// <param name="Odatacontext">Odatacontext</param>
/// <param name="Odatacount">Odatacount</param>
/// <param name="Value">Value</param>
public record CustomFieldDefinitions(string Odatacontext, float Odatacount, List<object> Value);

/// <summary>The structure that defines a Custom Field in Karbon</summary>
/// <param name="Name">Name</param>
/// <param name="Type">Type</param>
/// <param name="IsVisibleToContacts">IsVisibleToContacts</param>
/// <param name="IsVisibleToOrganizations">IsVisibleToOrganizations</param>
/// <param name="ListOptions">ListOptions</param>
public record CustomFieldDefinition(string Name, string Type, bool IsVisibleToContacts, bool IsVisibleToOrganizations, List<string> ListOptions);

/// <summary>A collection of Custom Fields</summary>
/// <param name="Key">Key</param>
/// <param name="Name">Name</param>
/// <param name="Type">Type</param>
/// <param name="Value">Value</param>
public record CustomFieldValue(string Key, string Name, string Type, List<string> Value);

/// <summary>The set custom fields for a contact or organization and the associated values and options</summary>
/// <param name="EntityKey">EntityKey</param>
/// <param name="CustomFieldValuesValue">CustomFieldValuesValue</param>
public record CustomFieldValues(string EntityKey, List<CustomFieldValue> CustomFieldValuesValue);

/// <summary>The details specific to the Accounting firm related to this entity</summary>
/// <param name="ContactPermaKey">A Karbon-generated key that is used to identify the Contact (if a person).</param>
/// <param name="OrganizationPermaKey">A Karbon-generated key that is used to identify the Contact (if a business or an organization).</param>
/// <param name="BirthDate">The day at which the contact (if a person) was born.</param>
/// <param name="DeathDate">The day at which the contact (if a person) died.</param>
/// <param name="Salutation">The title of the contact (if a person)</param>
/// <param name="Sex">The gender of the contact (if a person)</param>
/// <param name="FinancialYearEndDay">The day of month in which the fiscal year ends for the contact</param>
/// <param name="FinancialYearEndMonth">The month in which the fiscal year ends for the contact. Note that this is a 0 index value, so January is 0 and December is 11</param>
/// <param name="IncorporationDate">The date at which the contact (if a business or an organization) was incorporated.</param>
/// <param name="IncorporationState">The state in which the contact (if a business or an organization) was incorporated.</param>
/// <param name="LegalName">The legal name of the contact</param>
/// <param name="LineOfBusiness">The industry this contact is associated with.</param>
/// <param name="EntityType">The entity type of the contact (if a business or an organization)</param>
/// <param name="TaxCountryCode">The code of the country where the contact pays their tax.</param>
/// <param name="TradingName">The trading name of the contact (if a business or an organization)</param>
/// <param name="AnnualRevenue">The total annual revenue generated by the contact</param>
/// <param name="BaseCurrency">The 3-letter code, denoting the currency of annual revenue</param>
/// <param name="GstBasis">The method of accounting for GST/BAS</param>
/// <param name="GstPeriod">The GST period</param>
/// <param name="IncomeTaxInstallmentPeriod">The income tax installment period</param>
/// <param name="IsVATRegistered">Set to true if the business or organization is registered for VAT</param>
/// <param name="OrganizationValuation">The present value of the company</param>
/// <param name="PaysTax">Set to true, if the contact has to pay tax</param>
/// <param name="PrepareGST"><></param>
/// <param name="ProvisionalTaxBasic"><></param>
/// <param name="ProvisionalTaxRatio"><></param>
/// <param name="RevenueModel">The revenue model of the business or organization</param>
/// <param name="SalesTaxBasis">The tax method</param>
/// <param name="SalesTaxPeriod">The tax period</param>
/// <param name="Sells">The item that is primarily supplied by the company</param>
/// <param name="DateSignedEngagement"></param>
/// <param name="Bank">A user definable list of banks the client uses</param>
/// <param name="Payroll">A user definable list of payroll services the client uses</param>
/// <param name="BillPay">A user definable list of bill payment services the client uses</param>
/// <param name="Benefits">A user definable list of benefits services the client uses</param>
/// <param name="Revenue">A user definable list of revenue management services the client uses</param>
/// <param name="Expenses">A user definable list of expense management services the client uses</param>
/// <param name="FileManagement">A user definable list of file management services the client uses</param>
/// <param name="Accounting">A user definable list of accounting tools and services the client uses</param>
/// <param name="RegistrationNumbers">The registration numbers associated with the contact.</param>
/// <param name="LegalFirm">The legal firm(s) that represent the client, these can be strings or references to Karbon organizations</param>
/// <param name="TaxProvider">The provider of tax services to the client, these can be strings or references to Karbon organizations</param>
/// <param name="Notes">Notes</param>
public record DetailsOfAccounting(object ContactPermaKey, object OrganizationPermaKey, object BirthDate, object DeathDate, object Salutation, object Sex, object FinancialYearEndDay, object FinancialYearEndMonth, object IncorporationDate, object IncorporationState, object LegalName, object LineOfBusiness, object EntityType, object TaxCountryCode, object TradingName, object AnnualRevenue, object BaseCurrency, object GstBasis, object GstPeriod, object IncomeTaxInstallmentPeriod, object IsVATRegistered, object OrganizationValuation, object PaysTax, object PrepareGST, object ProvisionalTaxBasic, object ProvisionalTaxRatio, object RevenueModel, object SalesTaxBasis, object SalesTaxPeriod, object Sells, object DateSignedEngagement, List<string> Bank, List<string> Payroll, List<string> BillPay, List<string> Benefits, List<string> Revenue, List<string> Expenses, List<string> FileManagement, List<string> Accounting, List<object> RegistrationNumbers, object LegalFirm, object TaxProvider, List<object> Notes);

/// <summary>A comment on a Work Item, Note or Contact</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="CommentKey">A Karbon-generated unique identifier for the comment</param>
/// <param name="CommentTypeKey">A Karbon-generated value that is used to identify the Karbon entity (e.g., Work Item, Notes, Contacts) associated with the comment.</param>
/// <param name="CommentBody">The comment left behind by a Karbon user.</param>
/// <param name="CreatedDate">The date and time at which the comment was created in Karbon. This value is automatically generated by Karbon.  The datetime format is UTC.</param>
/// <param name="AuthorKey">A random value that indicates the email address of the Karbon user who created the comment.</param>
public record GetComment(string Odatacontext, string CommentKey, string CommentTypeKey, string CommentBody, string CreatedDate, string AuthorKey);

/// <summary>A list of Client Groups</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatacount">The total number of Client Groups that satisfy the query criteria.</param>
/// <param name="Value">Value</param>
/// <param name="OdatanextLink">The link to next set of Client Groups. @odata.nextLink field will only appear if you have more than 100 Client Groups.</param>
public record GetClientGroup(string Odatacontext, long Odatacount, List<object> Value, string OdatanextLink);

/// <summary>A list of Contacts</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatacount">The total number of Contacts that satisfy the query criteria.</param>
/// <param name="Value">Value</param>
/// <param name="OdatanextLink">The link to next set of Contacts. @odata.nextLink field will only appear if you have more than 100 Contacts.</param>
public record GetContacts(string Odatacontext, long Odatacount, List<object> Value, string OdatanextLink);

/// <summary>A collection of customer specific Contact and Work settings</summary>
/// <param name="ContactTypes">ContactTypes</param>
/// <param name="TenantKey">The unique key specific to the customer's Karbon account</param>
/// <param name="WorkStatuses">WorkStatuses</param>
/// <param name="WorkTypes">WorkTypes</param>
public record GetTenantSettings(List<SettingContactType> ContactTypes, string TenantKey, List<SettingWorkStatus> WorkStatuses, List<SettingWorkType> WorkTypes);

/// <summary>A list of Organizations</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatacount">The total number of Organizations that satisfy the query criteria.</param>
/// <param name="Value">Value</param>
/// <param name="OdatanextLink">The link to next set of Organizations. @odata.nextLink field will only appear if you have more than 100 Organizations.</param>
public record GetOrganizations(string Odatacontext, long Odatacount, List<object> Value, string OdatanextLink);

/// <summary>The Karbon Payment object</summary>
/// <param name="KarbonPayment">Data specific to Karbon Payments</param>
public record BillingKarbonPayment(object KarbonPayment);

/// <summary>The base data for a Work Item</summary>
/// <param name="WorkItemKey">A Karbon-generated unique identifier for the Work Item</param>
/// <param name="AssigneeEmailAddress">The email address of the user to whom the Work Item has been assigned</param>
/// <param name="AssigneeKey">A Karbon-generated value used to identify the user to whom the Work Item has been assigned</param>
/// <param name="AssigneeName">The name of the user to whom the Work Item has been assigned</param>
/// <param name="Title">The title of the Work Item</param>
/// <param name="ClientKey">A Karbon-generated value used to identify the Client for whom the Work Item is prepared</param>
/// <param name="ClientName">The name of the Client for whom the Work Item is prepared</param>
/// <param name="ClientType">The type of the Client</param>
/// <param name="ClientUserDefinedIdentifier">A unique identifier that you had created to identify the Client. This parameter is not case sensitive.</param>
/// <param name="RelatedClientGroupKey">A Karbon-generated value used to identify the Client Group of the Client</param>
/// <param name="ClientGroupKey">A Karbon-generated value used to identify the Client Group of the Client</param>
/// <param name="RelatedClientGroupName">The name of the related Client Group</param>
/// <param name="StartDate">The date and time at which the Work Item should start</param>
/// <param name="DueDate">The date and time by which the Work Item should be complete</param>
/// <param name="DeadlineDate">The date and time by which the Work Item should be complete according to a party outside of your firm. e.g. A tax filing date or payroll processing cut-off date.</param>
/// <param name="CompletedDate">The date and time at which the Work Item was completed</param>
/// <param name="ToDoPeriod">The period, in which the Work Item should start.</param>
/// <param name="WorkType">The type of work related to the Work Item</param>
/// <param name="WorkStatus">The current status of the Work Item</param>
/// <param name="PrimaryStatus">The current parent-level Status of the Work Item. If `WorkStatus` and `PrimaryStatus` have different values, `PrimaryStatus` will be the source of truth for Work Item status.</param>
/// <param name="SecondaryStatus">The current child-level Status of the Work Item</param>
/// <param name="WorkTemplateKey">A Karbon-generated value used to identify the Template that generated this Work Item</param>
/// <param name="WorkTemplateTile">The title of the Work Template</param>
/// <param name="WorkScheduleKey">A Karbon-generated value used to identify the Work Schedule for this Work Item</param>
/// <param name="EstimatedBudget">Deprecated! - previously used as the estimated budget for the Work Item</param>
public record WorkItem(string WorkItemKey, string AssigneeEmailAddress, string AssigneeKey, string AssigneeName, string Title, string ClientKey, string ClientName, string ClientType, string ClientUserDefinedIdentifier, string RelatedClientGroupKey, string ClientGroupKey, string RelatedClientGroupName, string StartDate, string DueDate, object DeadlineDate, string CompletedDate, string ToDoPeriod, string WorkType, string WorkStatus, string PrimaryStatus, string SecondaryStatus, string WorkTemplateKey, string WorkTemplateTile, string WorkScheduleKey, float EstimatedBudget);

/// <summary>A Work Item with Fee Settings</summary>

public record WorkItemWithFeeSettings();

/// <summary>A Work Item with Fee Settings and User Role assignments</summary>

public record WorkItemWithFeeSettingsAndUserRoleAssignments();

/// <summary>The core parameters when updating a Work Item</summary>
/// <param name="AssigneeEmailAddress">The email address of the user to whom the Work Item has been assigned</param>
/// <param name="Title">The title of the Work Item</param>
/// <param name="ClientKey">A Karbon-generated value used to identify the Client for whom the Work Item is prepared</param>
/// <param name="ClientType">The type of the Client</param>
/// <param name="RelatedClientGroupKey">A Karbon-generated value used to identify the Client Group of the Client</param>
/// <param name="StartDate">The date and time at which the Work Item should start</param>
/// <param name="DueDate">The date and time by which the Work Item should be complete</param>
/// <param name="DeadlineDate">The date and time by which an external party has specified that the Work Item should be complete</param>
/// <param name="WorkType">The type of work related to the Work Item. Enter an existing Work Type.</param>
/// <param name="WorkStatus">The current status of the Work Item</param>
/// <param name="WorkTemplateKey">A Karbon-generated value used to identify the Template that will be used to generate this Work Item</param>
/// <param name="WorkScheduleKey">A Karbon-generated value used to identify the Work Schedule for this Work Item</param>
/// <param name="EstimatedBudget">The estimated budget for the Work Item</param>
/// <param name="Description">A free form text field to add more information about the Work Item</param>
/// <param name="ClientTaskRecipient">The details about the recipient of the Client Tasks related to this Work Item.</param>
public record WorkItemUpdate(string AssigneeEmailAddress, string Title, string ClientKey, string ClientType, string RelatedClientGroupKey, string StartDate, string DueDate, object DeadlineDate, string WorkType, string WorkStatus, string WorkTemplateKey, string WorkScheduleKey, float EstimatedBudget, string Description, object ClientTaskRecipient);

/// <summary>The Fee Settings for a Work Item</summary>
/// <param name="FeeSettings">The Fee Settings describe how the work will be billed, for FixedFee work the Fee Settings can include the value to be billed</param>
public record WorkFeeSettings(object FeeSettings);

/// <summary>The User assignments for specific Roles on a Work Item</summary>
/// <param name="UserRoleAssignments">The array that describes the roles of the users associated with this Work Item.</param>
public record WorkItemUserRoleAssignments(List<object> UserRoleAssignments);

/// <summary>A collection of work items</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatacount">The total number of Work Items that satisfy the query criteria.</param>
/// <param name="Value">Value</param>
/// <param name="OdatanextLink">The link to next set of Work Items. @odata.nextLink field will only appear if you have more than 100 Work Items.</param>
public record GetWorkItems(string Odatacontext, long Odatacount, List<object> Value, string OdatanextLink);

/// <summary>The properties that can be updated on a Work Item with the PATCH method</summary>

public record UpdateWorkItem();

/// <summary>A single work item</summary>

public record RequestCreateWorkItem();

/// <summary>A single Client Group</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatatype">The information about Karbon Objects generating this reponse.</param>
/// <param name="ClientGroupKey">A Karbon-generated value that is used to identify the Client Group</param>
/// <param name="FullName">The full name of the Client Group</param>
/// <param name="ClientOwner">The team member in your firm who looks after client relationship or is responsible for the vast majority of the work of the clients in this Client Group. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ClientManager">The team member in your firm who manages the work for the clients in this Client Group. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ContactType">The Contact Type for this Client Group. You can only use existing Contact Type.</param>
/// <param name="UserDefinedIdentifier">A unique key that you can use to identify this Client Group.</param>
/// <param name="RestrictionLevel">The privacy level for this Client Group.  <ul><li>Public Client Groups can be managed by anyone in the firm</li> <li>Private Client Groups can only be managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team). Basic details (name of the Contacts and the Work Items) related to such Client Groups are visible to anyone in the firm.</li> <li>Hidden Client Groups are only be visible to and managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team).</li></param>
/// <param name="PrimaryContact">The main contact for a client with multiple contact details.</param>
/// <param name="LastModifiedDateTime">The date and time at which this Client Group was last modified.</param>
/// <param name="Members">The members of the Client Group</param>
/// <param name="EntityDescription">EntityDescription</param>
public record ResponseCreateClientGroup(string Odatacontext, string Odatatype, string ClientGroupKey, string FullName, string ClientOwner, string ClientManager, string ContactType, string UserDefinedIdentifier, string RestrictionLevel, string PrimaryContact, string LastModifiedDateTime, List<object> Members, object EntityDescription);

/// <summary>A single Client Group</summary>
/// <param name="FullName">The full name of the Client Group</param>
/// <param name="ClientOwner">The team member in your firm who looks after client relationship or is responsible for the vast majority of the work of the clients in this Client Group. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ClientManager">The team member in your firm who manages the work for the clients in this Client Group. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ContactType">The Contact Type for this Client Group. You can only use existing Contact Type.</param>
/// <param name="UserDefinedIdentifier">A unique key that you can use to identify this Client Group.</param>
/// <param name="RestrictionLevel">The privacy level for this Client Group.  <ul><li>Public Client Groups can be managed by anyone in the firm</li> <li>Private Client Groups can only be managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team). Basic details (name of the Contacts and the Work Items) related to such Client Groups are visible to anyone in the firm.</li> <li>Hidden Client Groups are only be visible to and managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team).</li></param>
/// <param name="PrimaryContact">The main contact for a client with multiple contact details.</param>
/// <param name="EntityDescription">EntityDescription</param>
/// <param name="Members">The members of the Client Group</param>
public record CreateClientGroup(string FullName, string ClientOwner, string ClientManager, string ContactType, string UserDefinedIdentifier, string RestrictionLevel, string PrimaryContact, object EntityDescription, List<object> Members);

/// <summary>A single Contact</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatatype">The information about Karbon Objects generating this reponse.</param>
/// <param name="ContactKey">A Karbon-generated value that is used to identify the Contact</param>
/// <param name="FirstName">The first name of the Contact</param>
/// <param name="MiddleName">The middle name of the Contact</param>
/// <param name="LastName">The last name of the Contact</param>
/// <param name="PreferredName">The preferred name of the Contact</param>
/// <param name="Salutation">The title to address the Contact</param>
/// <param name="Suffix">The suffix of the Contact</param>
/// <param name="ClientOwner">The team member in your firm who looks after client relationship or is responsible for the vast majority of the work of the Contact. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ClientManager">The team member in your firm who manages the work for Contact. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ContactType">The Contact Type for this Contact. You can only use existing Contact Types, these are available from the TenantSettings endpoint.</param>
/// <param name="UserDefinedIdentifier">A unique key that you can use to identify this Contact.</param>
/// <param name="RestrictionLevel">The privacy level for this Client Group.  <ul><li>Public Client Groups can be managed by anyone in the firm</li> <li>Private Client Groups can only be managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team). Basic details (name of the Contacts and the Work Items) related to such Client Groups are visible to anyone in the firm.</li> <li>Hidden Client Groups are only be visible to and managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team).</li></param>
/// <param name="AvatarUrl">The URL to the Contact's avatar</param>
/// <param name="LastModifiedDateTime">The date and time at which this Contact was last modified.</param>
/// <param name="EntityDescription">EntityDescription</param>
/// <param name="AccountingDetail">AccountingDetail</param>
/// <param name="BusinessCards">BusinessCards</param>
/// <param name="ClientTeam">ClientTeam</param>
public record ResponseCreateContact(string Odatacontext, string Odatatype, string ContactKey, string FirstName, string MiddleName, string LastName, string PreferredName, string Salutation, string Suffix, string ClientOwner, string ClientManager, string ContactType, string UserDefinedIdentifier, string RestrictionLevel, string AvatarUrl, string LastModifiedDateTime, object EntityDescription, DetailsOfAccounting AccountingDetail, BusinessCardsRequestAsArray BusinessCards, ClientTeam ClientTeam);

/// <summary>ContactsCardList</summary>

public record ContactsCardList();

/// <summary>A contact related to an Organization</summary>
/// <param name="ContactKey">A Karbon-generated value that is used to identify the Contact</param>
/// <param name="FullName">The full name of the Contact</param>
/// <param name="PreferredName">The preferred name of the Contact</param>
/// <param name="Salutation">The title to address the Contact</param>
/// <param name="ClientOwner">The team member in your firm who looks after client relationship or is responsible for the vast majority of the work of the Contact. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ClientManager">The team member in your firm who manages the work for Contact. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="Address">The Address of the Contact</param>
/// <param name="EmailAddress">The Email Address of the Contact</param>
/// <param name="PhoneNumber">The phone number associated with the Contact</param>
/// <param name="RoleOrTitle">The business role or title of the Contact</param>
/// <param name="UserDefinedIdentifier">A unique key that you can use to identify this Contact.</param>
/// <param name="LastModifiedDateTime">The date and time at which this Contact was last modified.</param>
public record ContactsCardOrganization(string ContactKey, string FullName, string PreferredName, string Salutation, object ClientOwner, object ClientManager, object Address, string EmailAddress, object PhoneNumber, string RoleOrTitle, string UserDefinedIdentifier, string LastModifiedDateTime);

/// <summary>A single Contact</summary>
/// <param name="FirstName">The first name of the Contact</param>
/// <param name="MiddleName">The middle name of the Contact</param>
/// <param name="LastName">The last name of the Contact</param>
/// <param name="PreferredName">The preferred name of the Contact</param>
/// <param name="Salutation">The title to address the Contact</param>
/// <param name="Suffix">The suffix of the Contact</param>
/// <param name="RestrictionLevel">The privacy level for this Contact.  <ul><li>Public Contacts can be managed by anyone in the firm</li> <li>Private Contacts can only be managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team). Basic details (name of the Contacts and the Work Items) related to such Contacts are visible to anyone in the firm.</li> <li>Hidden Contacts are only be visible to and managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team).</li></param>
/// <param name="ClientOwner">The team member in your firm who looks after client relationship or is responsible for the vast majority of the work of the Contact. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ClientManager">The team member in your firm who manages the work for Contact. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ContactType">The Contact Type for this Contact. You can only use existing Contact Types, these are available from the TenantSettings endpoint.</param>
/// <param name="UserDefinedIdentifier">A unique key that you can use to identify this Contact.</param>
/// <param name="BusinessCards">BusinessCards</param>
/// <param name="EntityDescription">EntityDescription</param>
/// <param name="AccountingDetail">AccountingDetail</param>
/// <param name="ClientTeam">ClientTeam</param>
public record CreateContact(string FirstName, string MiddleName, string LastName, string PreferredName, string Salutation, string Suffix, string RestrictionLevel, string ClientOwner, string ClientManager, string ContactType, string UserDefinedIdentifier, List<BusinessCardRequest> BusinessCards, object EntityDescription, DetailsOfAccounting AccountingDetail, ClientTeam ClientTeam);

/// <summary>A single Note</summary>
/// <param name="Subject">The subject line of the Note</param>
/// <param name="Body">The body of the Note. This property supports HTML.</param>
/// <param name="AuthorEmailAddress">The email address of the Note's author</param>
/// <param name="AssigneeEmailAddress">The email address of the Note's reader</param>
/// <param name="DueDate">The date by which the actions on the note must be complete</param>
/// <param name="TodoDate">The date for which the actions will be executed</param>
/// <param name="Timelines">The Timelines to which this note will be associated. If Notes are linked to a Work Item, the Timeline of the Client (associated with the Work Item) will show the Notes.</param>
public record NoteRequest(string Subject, string Body, string AuthorEmailAddress, string AssigneeEmailAddress, string DueDate, string TodoDate, List<object> Timelines);

/// <summary>A single Contact</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response</param>
/// <param name="Id">A Karbon-generated value that is used to identify the Note</param>
/// <param name="Subject">The subject line of the Note</param>
/// <param name="Body">The body of the Note. This property supports HTML.</param>
/// <param name="AuthorEmailAddress">The email address of the Note's author</param>
/// <param name="AssigneeEmailAddress">The email address of the Note's reader</param>
/// <param name="DueDate">The date by which the actions on the note must be complete</param>
/// <param name="TodoDate">The date for which the actions will be executed</param>
/// <param name="Timelines">The Timelines to which this note will be associated. If Notes are linked to a Work Item, the Timeline of the Client (associated with the Work Item) will show the Notes.</param>
/// <param name="Comments">The comments for this note</param>
public record NoteResponse(string Odatacontext, string Id, string Subject, string Body, string AuthorEmailAddress, string AssigneeEmailAddress, string DueDate, string TodoDate, List<object> Timelines, List<object> Comments);

/// <summary>A single Organization</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatatype">The information about Karbon Objects generating this reponse.</param>
/// <param name="OrganizationKey">A Karbon-generated value that is used to identify the Organization</param>
/// <param name="FullName">The full name of the Organization</param>
/// <param name="ClientOwner">The team member in your firm who looks after client relationship or is responsible for the vast majority of the work of the clients in this Organization. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ClientManager">The team member in your firm who manages the work for the clients in this Organization. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ContactType">The Contact Type for this Organization. You can only use existing Contact Type.</param>
/// <param name="UserDefinedIdentifier">A unique key that you can use to identify this Organization.</param>
/// <param name="RestrictionLevel">The privacy level for this Organization.  <ul><li>Public Organization can be managed by anyone in the firm</li> <li>Private Organization can only be managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team). Basic details (name of the Organization and the Work Items) related to such Organization are visible to anyone in the firm.</li> <li>Hidden Organization are only be visible to and managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team).</li></param>
/// <param name="LastModifiedDateTime">The date and time at which this Contact was last modified.</param>
/// <param name="EntityDescription">EntityDescription</param>
/// <param name="AccountingDetail">AccountingDetail</param>
public record CreateOrganizationResponse(string Odatacontext, string Odatatype, string OrganizationKey, string FullName, string ClientOwner, string ClientManager, string ContactType, string UserDefinedIdentifier, string RestrictionLevel, string LastModifiedDateTime, object EntityDescription, DetailsOfAccounting AccountingDetail);

/// <summary>A single Organization</summary>
/// <param name="FullName">The full name of the Organization</param>
/// <param name="ClientOwner">The team member in your firm who looks after client relationship or is responsible for the vast majority of the work of the clients in this Organization. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ClientManager">The team member in your firm who manages the work for the clients in this Organization. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ContactType">The Contact Type for this Organization. You can only use existing Contact Type.</param>
/// <param name="UserDefinedIdentifier">A unique key that you can use to identify this Organization.</param>
/// <param name="RestrictionLevel">The privacy level for this Organization.  <ul><li>Public Organization can be managed by anyone in the firm</li> <li>Private Organization can only be managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team). Basic details (name of the Organization and the Work Items) related to such Organization are visible to anyone in the firm.</li> <li>Hidden Organization are only be visible to and managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team).</li></param>
/// <param name="BusinessCards">BusinessCards</param>
/// <param name="EntityDescription">EntityDescription</param>
/// <param name="AccountingDetail">AccountingDetail</param>
/// <param name="ClientTeam">ClientTeam</param>
public record CreateOrganization(string FullName, string ClientOwner, string ClientManager, string ContactType, string UserDefinedIdentifier, string RestrictionLevel, List<BusinessCardRequest> BusinessCards, object EntityDescription, DetailsOfAccounting AccountingDetail, ClientTeam ClientTeam);

/// <summary>A single Organization</summary>
/// <param name="FullName">The full name of the Organization</param>
/// <param name="ClientOwner">The team member in your firm who looks after client relationship or is responsible for the vast majority of the work of the clients in this Organization. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ClientManager">The team member in your firm who manages the work for the clients in this Organization. You can only use the UserKey or the email address of an existing team member.</param>
/// <param name="ContactType">The Contact Type for this Organization. You can only use existing Contact Type.</param>
/// <param name="UserDefinedIdentifier">A unique key that you can use to identify this Organization.</param>
/// <param name="RestrictionLevel">The privacy level for this Organization.  <ul><li>Public Organization can be managed by anyone in the firm</li> <li>Private Organization can only be managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team). Basic details (name of the Organization and the Work Items) related to such Organization are visible to anyone in the firm.</li> <li>Hidden Organization are only be visible to and managed by the [Client Team](https://help.karbonhq.com/en/articles/1524707-create-a-client-team).</li></param>
/// <param name="AccountingDetail">AccountingDetail</param>
/// <param name="BusinessCards">BusinessCards</param>
/// <param name="EntityDescription">EntityDescription</param>
/// <param name="ClientTeam">ClientTeam</param>
public record UpdateOrganization(string FullName, string ClientOwner, string ClientManager, string ContactType, string UserDefinedIdentifier, string RestrictionLevel, DetailsOfAccounting AccountingDetail, List<BusinessCardRequest> BusinessCards, object EntityDescription, ClientTeam ClientTeam);

/// <summary>The response returned when a business card is created and $expand=BusinessCards is specified in the URL</summary>

public record WithBusinessCardsCreateOrganizationResponse();

/// <summary>A collection of files associated with an entity in Karbon</summary>
/// <param name="EntityKey">The unique key of the entity the list of files is associated with</param>
/// <param name="EntityType">The type of entity the file list is associated with</param>
/// <param name="Attachments">An array of Files including a name, mimetype, size, creation timestamp and download url</param>
public record FileList(string EntityKey, string EntityType, List<FileListItem> Attachments);

/// <summary>A single File</summary>
/// <param name="FileContextKey">The unique key of the file</param>
/// <param name="FileName">The name of the file</param>
/// <param name="FileSize">The size of the file in bytes</param>
/// <param name="MimeType">The mimetype of the uploaded file</param>
/// <param name="DownloadUrl">The API path than can be called to download the file</param>
/// <param name="DateCreated">The timestamp of when the file was created</param>
public record FileListItem(string FileContextKey, string FileName, int FileSize, string MimeType, string DownloadUrl, string DateCreated);

/// <summary>A single TimeSheet with TimeEntries</summary>
/// <param name="TimesheetKey">A Karbon-generated unique identifier for the Timesheet</param>
/// <param name="StartDate">The start date for the TimeSheet</param>
/// <param name="EndDate">The end date for the TimeSheet</param>
/// <param name="UserKey">A Karbon-generated key used to identify the user, this Timesheet belongs to</param>
/// <param name="Status">The approval status of the Timesheet</param>
/// <param name="WorkItemKeys">An array of WorkItem keys</param>
/// <param name="TimeEntries">An array of Time entries</param>
public record GetSingleTimeSheet(string TimesheetKey, string StartDate, string EndDate, string UserKey, string Status, List<string> WorkItemKeys, List<object> TimeEntries);

/// <summary>A collection of TimeSheets</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatacount">The total number of Timesheets that satisfy the query criteria.</param>
/// <param name="Value">Value</param>
/// <param name="OdatanextLink">The link to next set of Timesheets. @odata.nextLink field will only appear if you have more than 100 Timesheets.</param>
public record GetTimeSheets(string Odatacontext, long Odatacount, List<GetSingleTimeSheet> Value, string OdatanextLink);

/// <summary>The unique id, name and email address for a single User</summary>
/// <param name="Id">A Karbon-generated key used to identify the User</param>
/// <param name="Name">The name of the User</param>
/// <param name="EmailAddress">The email address of the User</param>
public record GetSingleUser(string Id, string Name, string EmailAddress);

/// <summary>The details for a single User</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatatype">The information about Karbon data type in this response.</param>
/// <param name="Id">The unique Id of the User, this should match the key used in the URL</param>
/// <param name="Name">The name of the User</param>
/// <param name="EmailAddress">The email address of the User</param>
/// <param name="BillableRate">The default hourly rate in at which the user's work is billed at. The currency for `BillableRate` is based on the firm's currency and region found in Karbon under Settings > Account Settings.</param>
/// <param name="CapacityMinutesPerWeek">The number of billable minutes allocated to the user per week</param>
/// <param name="Permissions">A list of the permissions allocated to the User</param>
/// <param name="Roles">A list of the roles allocated to the User</param>
/// <param name="Teams">A list of the teams the User is member of</param>
public record GetUserById(string Odatacontext, string Odatatype, string Id, string Name, string EmailAddress, double BillableRate, int CapacityMinutesPerWeek, List<string> Permissions, List<string> Roles, List<string> Teams);

/// <summary>A collection of Users</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatacount">The total number of Users that satisfy the query criteria.</param>
/// <param name="Value">Value</param>
/// <param name="OdatanextLink">The link to next set of Users. @odata.nextLink field will only appear if you have more than 100 Users.</param>
public record GetUsers(string Odatacontext, long Odatacount, List<GetSingleUser> Value, string OdatanextLink);

/// <summary>The details of a specific Webhook Subscription</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response</param>
/// <param name="TargetUrl">The URL to your server that is waiting to receive information about the `WebhookType` from Karbon</param>
/// <param name="WebhookType">The type of Karbon entity, which the Webhook Subscription is associated with</param>
public record GetWebhookSubscriptions(string Odatacontext, string TargetUrl, string WebhookType);

/// <summary>A single Work Schedule</summary>

public record WorkSchedulePUTRequest();

/// <summary>A single Work Schedule</summary>
/// <param name="CreatedFromWorkItemKey">The key of the Work Item, which the Work Schedule will be assigned to. This value will be `null`.</param>
/// <param name="RecurrenceFrequency">The unit of time at which this Work Schedule should repeat. Once set, the value of this property can not be changed.</param>
/// <param name="CustomFrequencyUnits">The unit of time at which this Work Schedule should repeat. Once set, the value of this property can not be changed.  This property is only applicable if `RecurrenceFrequency` is set to `Custom`.</param>
/// <param name="CustomFrequencyMultiple">The multiple of time unit at which this Work Schedule should repeat.  This property is not applicable if `RecurrenceFrequency` is set to `SemiMonth`, `Fortnight`, or `Quarter`.</param>
/// <param name="ScheduleStartDate">The date at which this Work Schedule should start running</param>
/// <param name="ScheduleEndDate">The date at which this Work Schedule should stop running  To set the Work Schedule to run forever, set the value of this property to `null`.</param>
/// <param name="ScheduleDueDateMethod">The method that should be used to count the Due Date for the Work Item</param>
/// <param name="ScheduleDueDateDays">The value of the count for the Due Date  This property is not applicable if `ScheduleDueDateMethod` is set to `LastDayOfMonth` or `NoDueDate`.  Example of usage: <table> <thead> <th>ScheduleDueDateMethod</th> <th>ScheduleDueDateDays</th> <th>Read Due Date as</th> </thead> <tbody> <tr> <td>DaysFromStartDate</td> <td>7</td> <td>7 days after start date</td> </tr> <tr> <td>DayOfMonth</td> <td>7</td> <td>7th day of the current month</td> </tr> <tr> <td>LastDayOfMonth</td> <td>N/A</td> <td>Last day of the current month</td> </tr> <tr> <td>DayOfNextMonth</td> <td>7</td> <td>7th day of the next month</td> </tr> <tr> <td>NoDueDate</td> <td>N/A</td> <td>Work has no due date</td> </tr> </tbody> </table></param>
/// <param name="ScheduleDueDateMonthMultiple">The value to denote the number of the months, if the due date is after 2 months.  This property is only applicable if `ScheduleDueDateMethod` is set to `DayOfSubsequentMonth`.  Example of usage: <table> <thead> <th>ScheduleDueDateMethod</th> <th>ScheduleDueDateDays</th> <th>ScheduleDueDateMonthMultiple</th> <th>Read Due Date as</th> </thead> <tbody> <tr> <td>DaysFromStartDate</td> <td>7</td> <td>3</td> <td>7th day of the 3rd month after start date</td> </tr> </tbody> </table></param>
/// <param name="PreventStartEndOnWeekend">If the value of this property is set to `true`, the Work Item will not start on Weekends.</param>
/// <param name="InitializeBeforeStartDateUnits">You can automatically create a Work Item before its Start date. Use this property to set the unit of delay between Start date and Work Item creation date.</param>
/// <param name="InitializeBeforeStartDateMultiple">You can automatically create a Work Item before its Start date. Use this property to set the multiple of delay unit between Start date and Work Item creation date.</param>
/// <param name="InitializeTasksBeforeStartDateUnits">You can automatically create budget and tasks for a Work Item before its Start date. Use this property to set the unit of delay between Start date and task creation date.</param>
/// <param name="InitializeTasksBeforeStartDateMultiple">You can automatically create budget and tasks for a Work Item before its Start date. Use this property to set the multiple of delay unit between Start date and task creation date.</param>
/// <param name="WorkItemTitleDefinition">Use this property to define the format of the Work Item title. Karbon offers a series of object that you can use to construct Work Item Title. For more information, see Technical Guide to Work Item Title Definition.</param>
public record WorkScheduleRequest(string CreatedFromWorkItemKey, string RecurrenceFrequency, object CustomFrequencyUnits, int CustomFrequencyMultiple, string ScheduleStartDate, string ScheduleEndDate, string ScheduleDueDateMethod, int ScheduleDueDateDays, int ScheduleDueDateMonthMultiple, bool PreventStartEndOnWeekend, string InitializeBeforeStartDateUnits, int InitializeBeforeStartDateMultiple, string InitializeTasksBeforeStartDateUnits, int InitializeTasksBeforeStartDateMultiple, string WorkItemTitleDefinition);

/// <summary>A single Work Schedule</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="WorkScheduleKey">A Karbon-generated unique identifier for the Work Schedule</param>
/// <param name="ClientKey">A Karbon-generated value used to identify the Client for whom the Work Item is prepared</param>
/// <param name="ClientType">The type of the Client</param>
/// <param name="CreatedFromWorkItemKey">The key of the Work Item, which the Work Schedule will be assigned to. This value will be `null`.</param>
/// <param name="RecurrenceFrequency">The denominator of time period at which this Work Schedule should repeat. Once set, the value of this property can not be changed.</param>
/// <param name="CustomFrequencyUnits">The unit of time at which this Work Schedule should repeat. Once set, the value of this property can not be changed.  This property is only applicable if `RecurrenceFrequency` is set to `Custom`.</param>
/// <param name="CustomFrequencyMultiple">The multiple of time unit at which this Work Schedule should repeat.  This property is not applicable if `RecurrenceFrequency` is set to `SemiMonth`, `Fortnight`, or `Quarter`.</param>
/// <param name="FrequencyDescription">A description of the Frequency</param>
/// <param name="ScheduleStartDate">The date at which this Work Schedule should start running</param>
/// <param name="ScheduleEndDate">The date at which this Work Schedule should stop running  To set the Work Schedule to run forever, set the value of this property to `0001-01-01`.</param>
/// <param name="ScheduleDueDateMethod">The method that should be used to count the Due Date for the Work Item</param>
/// <param name="ScheduleDueDateDays">The value of the count for the Due Date  This property is not applicable if `ScheduleDueDateMethod` is set to `LastDayOfMonth` or `NoDueDate`.  Example of usage:  <table> <th>ScheduleDueDateMethod</th> <th>ScheduleDueDateDays</th> <th>Read Due Date as</th> <tr> <td>DaysFromStartDate</td> <td>7</td> <td>7 days after start date</td> </tr> <tr> <td>DayOfMonth</td> <td>7</td> <td>7th day of the current month</td> </tr> <tr> <td>LastDayOfMonth</td> <td>N/A</td> <td>Last day of the current month</td> </tr> <tr> <td>DayOfNextMonth</td> <td>7</td> <td>7th day of the next month</td> </tr> <tr> <td>NoDueDate</td> <td>N/A</td> <td>Work has no due date</td> </tr> </table></param>
/// <param name="ScheduleDueDateMonthMultiple">The value to denote the number of the months, if the due date is after 2 months.  This property is only applicable if `ScheduleDueDateMethod` is set to `DayOfSubsequentMonth`.  Example of usage: <table> <thead> <th>ScheduleDueDateMethod</th> <th>ScheduleDueDateDays</th> <th>ScheduleDueDateMonthMultiple</th> <th>Read Due Date as</th> </thead> <tbody> <tr> <td>DaysFromStartDate</td> <td>7</td> <td>3</td> <td>7th day of the 3rd month after start date</td> </tr> </tbody> </table></param>
/// <param name="AssigneeUserKey">A Karbon-generated unique identifier to indicate the user, whom the Work Item has been assigned to</param>
/// <param name="WorkItemDescription">A free form text field for more information about the Work Item</param>
/// <param name="PreventStartEndOnWeekend">If the value of this property is set to `true`, the Work Item will not start on Weekends.</param>
/// <param name="InitializeBeforeStartDateUnits">You can automatically create a Work Item before its Start date. Use this property to set the unit of delay between Start date and Work Item creation date.</param>
/// <param name="InitializeBeforeStartDateMultiple">You can automatically create a Work Item before its Start date. Use this property to set the multiple of delay unit between Start date and Work Item creation date.</param>
/// <param name="WorkItemTitleDefinition">Use this property to define the format of the Work Item title. Karbon offers a series of object that you can use to construct Work Item Title. For more information, see Technical Guide to Work Item Title Definition.</param>
/// <param name="WorkItemTitleDisplayText">The title of the related Work Item</param>
/// <param name="InitializeTasksBeforeStartDateUnits">You can automatically create budget and tasks for a Work Item before its Start date. Use this property to set the unit of delay between Start date and task creation date.</param>
/// <param name="InitializeTasksBeforeStartDateMultiple">You can automatically create budget and tasks for a Work Item before its Start date. Use this property to set the multiple of delay unit between Start date and task creation date.</param>
public record GetWorkSchedule(string Odatacontext, string WorkScheduleKey, string ClientKey, string ClientType, string CreatedFromWorkItemKey, string RecurrenceFrequency, object CustomFrequencyUnits, int CustomFrequencyMultiple, string FrequencyDescription, string ScheduleStartDate, string ScheduleEndDate, string ScheduleDueDateMethod, int ScheduleDueDateDays, int ScheduleDueDateMonthMultiple, string AssigneeUserKey, string WorkItemDescription, bool PreventStartEndOnWeekend, string InitializeBeforeStartDateUnits, int InitializeBeforeStartDateMultiple, string WorkItemTitleDefinition, string WorkItemTitleDisplayText, string InitializeTasksBeforeStartDateUnits, int InitializeTasksBeforeStartDateMultiple);

/// <summary>A single Work Template</summary>
/// <param name="WorkTemplateKey">A Karbon-generated unique identifier for the Work Template</param>
/// <param name="Title">The title of the Work Template</param>
/// <param name="Description">A free text field to add more information about the Work Template</param>
/// <param name="WorkTypeKey">A Karbon-generated unique identifier to denote the Work Type</param>
/// <param name="HasScheduledClientTaskGroups">This property is set to true, if the Work Template has scheduled Client Tasks</param>
/// <param name="EstimatedBudget">The estimated budget for the Work Items generated using this Work Template. This property value will be `null`.</param>
/// <param name="EstimatedTime">The estimated time required to complete the Work Items generated using this Work Template. This property value will be `null`.</param>
/// <param name="DraftHasChanges">This property is set to true, if the Work Template has unpublished changes</param>
/// <param name="PublishedDate">The date at which this Work Template was published</param>
/// <param name="NumberOfWorkItemsCreated">The number of Work Items created using this Work Template</param>
/// <param name="DateLastWorkItemCreated">The most recent date at which a Work Item was created using this Work Template</param>
/// <param name="DateModified">The most recent date at which this Work Template was modified</param>
/// <param name="ActorRoles">The user roles who are required to action the Work Items created using this Work Template</param>
public record GetSingleWorkTemplate(string WorkTemplateKey, string Title, string Description, string WorkTypeKey, bool HasScheduledClientTaskGroups, object EstimatedBudget, string EstimatedTime, bool DraftHasChanges, string PublishedDate, float NumberOfWorkItemsCreated, string DateLastWorkItemCreated, string DateModified, List<object> ActorRoles);

/// <summary>A collection of Work Templates</summary>
/// <param name="Odatacontext">The information about Karbon controllers generating this response.</param>
/// <param name="Odatacount">The total number of Work Templates that satisfy the query criteria.</param>
/// <param name="Value">Value</param>
/// <param name="OdatanextLink">The link to next set of Work Templates. @odata.nextLink field will only appear if you have more than 10 Work Templates.</param>
public record GetWorkTemplates(string Odatacontext, long Odatacount, List<GetSingleWorkTemplate> Value, string OdatanextLink);

/// <summary>The details of an error associated with an API request</summary>
/// <param name="Error">Error</param>
public record ErrorMessages(object Error);

/// <summary>The details of an error associated with an API request</summary>
/// <param name="Error">Error</param>
public record EnhancedErrorMessages(object Error);

/// <summary>The error message returned when the API rate limit is hit</summary>
/// <param name="StatusCode">The generic HTTP Error code</param>
/// <param name="Message">The error message</param>
public record RateLimitErrorMessage(object StatusCode, string Message);

/// <summary>The error message returned when the API rate limit is hit</summary>
/// <param name="Error">The error object</param>
public record ManualPaymentNotFoundErrorMessage(object Error);

/// <summary>A generic response shown when the API cannot find a requested entity</summary>
/// <param name="StatusCode">The generic HTTP Error code</param>
/// <param name="Message">The error message</param>
public record ResourceNotFound(object StatusCode, string Message);

/// <summary>The name and unique key for a Contact Type</summary>
/// <param name="Name">The name assigned to the Customer Type</param>
/// <param name="ContactTypeKey">The unique key assigned to the Customer Type</param>
public record SettingContactType(string Name, string ContactTypeKey);

/// <summary>A Work Status, its child statuses with Name, Type and unique Key</summary>
/// <param name="Name">The name assigned the Work Status</param>
/// <param name="Type">The type of status, expected to be Primary at this level</param>
/// <param name="Children">Children</param>
public record SettingWorkStatus(string Name, string Type, List<object> Children);

/// <summary>The schema for a single Work Type entry</summary>
/// <param name="Name">The name assigned to the Work Type</param>
/// <param name="WorkTypeKey">The unique key assigned to the Work Type</param>
/// <param name="AvailableStatuses">AvailableStatuses</param>
public record SettingWorkType(string Name, string WorkTypeKey, object AvailableStatuses);

/// <summary>A list of Integration Task Definitions</summary>
/// <param name="Odatacontext">Odatacontext</param>
/// <param name="Values">Values</param>
public record IntegrationTaskDefinitions(string Odatacontext, List<IntegrationTaskDefinition> Values);

/// <summary>A list of Integration Task Definitions</summary>
/// <param name="TaskDefinitionKey">The unique ID of the specific Integration Task Definition</param>
/// <param name="TaskName">The name of the specific Integration Task Definition</param>
/// <param name="TaskPrimaryLinkUrl">The optional default primary link URL of the specific Integration Task Definition</param>
/// <param name="TaskPrimaryLinkLabel">The optional default label for the primary link URL of the specific Integration Task Definition</param>
/// <param name="TaskSecondaryLinkUrl">The optional default label for the secondary link URL of the specific Integration Task Definition</param>
/// <param name="TaskSecondaryLinkLabel">The optional default label for the secondary link URL of the specific Integration Task Definition</param>
/// <param name="Locales">The list of locales and languages the integration task definition is suitable for use with, useful when different definitions are required for different languages or locales</param>
/// <param name="AllowedTaskUrlHosts">The list of allowed hostnames that the primary and secondary link URLs must match</param>
public record IntegrationTaskDefinition(string TaskDefinitionKey, string TaskName, object TaskPrimaryLinkUrl, object TaskPrimaryLinkLabel, string TaskSecondaryLinkUrl, string TaskSecondaryLinkLabel, List<string> Locales, List<string> AllowedTaskUrlHosts);

/// <summary>A list of Integration Tasks</summary>
/// <param name="Odatacontext">Odatacontext</param>
/// <param name="Odatacount">Odatacount</param>
/// <param name="Value">Value</param>
public record IntegrationTasks(string Odatacontext, float Odatacount, List<IntegrationTask> Value);

/// <summary>A single Integration Task</summary>

public record IntegrationTask();

/// <summary>The read-only properties of an Integration Task</summary>
/// <param name="CreatedAt">CreatedAt</param>
/// <param name="TaskDefinitionKey">The unique ID of the integration task definition</param>
/// <param name="TaskKey">The unique key of the specific Integration Task Definition for the associated Karbon account</param>
/// <param name="Locale">The list of locales and languages the integration task definition is suitable for use with, useful when different definitions are required for different languages or locales</param>
/// <param name="WorkItemKey">WorkItemKey</param>
/// <param name="WorkItemClientKey">WorkItemClientKey</param>
/// <param name="WorkType">WorkType</param>
public record IntegrationTaskReadOnly(string CreatedAt, string TaskDefinitionKey, string TaskKey, string Locale, string WorkItemKey, string WorkItemClientKey, string WorkType);

/// <summary>The writable properties of an Integration Task</summary>
/// <param name="TaskName">The name of the specific Integration Task Definition</param>
/// <param name="TaskPrimaryLinkUrl">The optional default primary link URL of the specific Integration Task Definition</param>
/// <param name="TaskPrimaryLinkLabel">The optional default label for the primary link URL of the specific Integration Task Definition</param>
/// <param name="TaskSecondaryLinkUrl">The optional default label for the secondary link URL of the specific Integration Task Definition</param>
/// <param name="TaskSecondaryLinkLabel">The optional default label for the secondary link URL of the specific Integration Task Definition</param>
/// <param name="TaskStatus">TaskStatus</param>
/// <param name="PrimaryStatus">PrimaryStatus</param>
/// <param name="TaskDueDate">TaskDueDate</param>
/// <param name="TaskData">TaskData</param>
public record IntegrationTaskWriteable(string TaskName, object TaskPrimaryLinkUrl, object TaskPrimaryLinkLabel, string TaskSecondaryLinkUrl, string TaskSecondaryLinkLabel, string TaskStatus, string PrimaryStatus, string TaskDueDate, List<IntegrationTaskData> TaskData);

/// <summary>Optional integration task data key/value pairs</summary>
/// <param name="Key">Key</param>
/// <param name="Value">Value</param>
public record IntegrationTaskData(string Key, string Value);

/// <summary>The schema for Contact webhook payloads</summary>
/// <param name="ResourcePermaKey">The key for the resource that has been created or updated</param>
/// <param name="ResourceType">The type of resource that has been created or updated</param>
/// <param name="ActionType">The action that triggered the webhook notifcation to be sent</param>
/// <param name="TimeStamp">The timestamp the associated resource was last altered</param>
public record ContactWebhook(string ResourcePermaKey, string ResourceType, string ActionType, string TimeStamp);

/// <summary>The schema for EstimateSummary webhook payloads</summary>
/// <param name="ResourcePermaKey">The key for the resource that has been created or updated</param>
/// <param name="ResourceType">The type of resource that has been created or updated</param>
/// <param name="ActionType">The action that triggered the webhook notifcation to be sent</param>
/// <param name="TimeStamp">The timestamp the associated resource was last altered</param>
public record EstimateSummaryWebhook(string ResourcePermaKey, string ResourceType, string ActionType, string TimeStamp);

/// <summary>The schema for Integration Task webhook payloads</summary>
/// <param name="ResourcePermaKey">The key for the resource that has been created or updated</param>
/// <param name="ResourceType">The type of resource that has been created or updated</param>
/// <param name="ActionType">The action that triggered the webhook notifcation to be sent</param>
/// <param name="TimeStamp">The timestamp the associated resource was last altered</param>
/// <param name="ClientKey">The key for the client related to the integration task</param>
/// <param name="ClientType">The type of client related to the integration task</param>
/// <param name="WorkItemKey">The key for the Work Item that the Integration Task was added to</param>
public record IntegrationTaskWebhook(string ResourcePermaKey, string ResourceType, string ActionType, string TimeStamp, string ClientKey, string ClientType, string WorkItemKey);

/// <summary>The schema for Invoice webhook payloads</summary>
/// <param name="ResourcePermaKey">The key for the resource that has been created or updated</param>
/// <param name="ResourceType">The type of resource that has been created or updated</param>
/// <param name="ActionType">The action that triggered the webhook notifcation to be sent</param>
/// <param name="TimeStamp">The timestamp the associated resource was last altered</param>
public record InvoiceWebhook(string ResourcePermaKey, string ResourceType, string ActionType, string TimeStamp);

/// <summary>The schema for Note webhook payloads</summary>
/// <param name="ResourcePermaKey">The key for the resource that has been created or updated</param>
/// <param name="ResourceType">The type of resource that has been created or updated</param>
/// <param name="ActionType">The action that triggered the webhook notifcation to be sent</param>
/// <param name="TimeStamp">The timestamp the associated resource was last altered</param>
public record NoteWebhook(string ResourcePermaKey, string ResourceType, string ActionType, string TimeStamp);

/// <summary>The schema for User webhook payloads</summary>
/// <param name="ResourcePermaKey">The key for the resource that has been created</param>
/// <param name="ResourceType">The type of resource that has been created</param>
/// <param name="ActionType">The action that triggered the webhook notifcation to be sent</param>
/// <param name="TimeStamp">The timestamp the associated resource was last altered</param>
public record UserWebhook(string ResourcePermaKey, string ResourceType, string ActionType, string TimeStamp);

/// <summary>The schema for Work webhook payloads</summary>
/// <param name="ResourcePermaKey">The key for the resource that has been created or updated</param>
/// <param name="ResourceType">The type of resource that has been created or updated</param>
/// <param name="ActionType">The action that triggered the webhook notifcation to be sent</param>
/// <param name="TimeStamp">The timestamp the associated resource was last altered</param>
public record WorkWebhook(string ResourcePermaKey, string ResourceType, string ActionType, string TimeStamp);