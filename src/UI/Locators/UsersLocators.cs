namespace AutomationFramework.UI.Locators;

/// <summary>
/// CSS selectors for the OrangeHRM Admin → User Management → Users page.
/// Verified against https://opensource-demo.orangehrmlive.com/web/index.php/admin/viewSystemUsers
/// </summary>
public static class UsersLocators
{
    // Table (initial list uses div-based rows; search results use a <table>)
    public const string UsersTable       = ".oxd-table, .oxd-table-body";
    public const string UserTableRows    = ".oxd-table-body .oxd-table-row, table tbody:last-of-type tr";
    public const string RecordsFoundText = ".orangehrm-horizontal-padding span, .orangehrm-container span";

    // Toolbar
    public const string AddUserButton    = "button:has-text('Add')";

    // Search / filter form
    public const string UsernameSearchInput  = ".oxd-table-filter input.oxd-input";
    public const string SearchButton         = "button:has-text('Search')";
    public const string ResetButton          = "button:has-text('Reset')";

    // Row actions — icon buttons inside each data row
    public const string EditButtonInRow      = ".oxd-table-cell-action-space button:first-child";
    public const string DeleteButtonInRow    = ".oxd-table-cell-action-space button:last-child";

    // Add/Edit user form
    public const string UserRoleSelectInForm = ".oxd-form-row .oxd-select-text-input";
    public const string EmployeeNameInput    = "input.oxd-autocomplete-text-input";
    public const string UsernameFormInput    = "input.oxd-input[autocomplete='off']";
    public const string StatusDropdown       = ".oxd-form-row .oxd-select-text:last-of-type";
    public const string PasswordFormInput    = "input[type='password']:first-of-type";
    public const string ConfirmPasswordInput = "input[type='password']:last-of-type";
    public const string SaveButton           = "button[type='submit']";
    public const string SuccessToast         = ".oxd-toast-content--success";
    public const string DeleteConfirmButton  = ".orangehrm-modal-footer button:has-text('Yes, Delete')";

    /// <summary>Edit button selector scoped to the row containing <paramref name="username"/>.</summary>
    public static string EditButtonForUser(string username) =>
        $".oxd-table-row:has-text('{username}') .oxd-table-cell-action-space button:first-child";

    /// <summary>Delete button selector scoped to the row containing <paramref name="username"/>.</summary>
    public static string DeleteButtonForUser(string username) =>
        $".oxd-table-row:has-text('{username}') .oxd-table-cell-action-space button:last-child";
}
