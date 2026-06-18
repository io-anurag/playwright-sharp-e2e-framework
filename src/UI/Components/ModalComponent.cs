using Microsoft.Playwright;

namespace AutomationFramework.UI.Components;

/// <summary>
/// Generic modal / dialog component.
/// Handles common modal interactions: open detection, confirm, cancel, close.
/// </summary>
public sealed class ModalComponent : BaseComponent
{
    private const string ModalOverlay  = "[data-testid='modal'], .modal.show, dialog[open], [role='dialog']";
    private const string ModalTitle    = "[data-testid='modal-title'], .modal-title, dialog h2";
    private const string ConfirmButton = "[data-testid='modal-confirm'], .modal .btn-primary, dialog button[type='submit']";
    private const string CancelButton  = "[data-testid='modal-cancel'], .modal .btn-secondary, dialog button[type='button']";
    private const string CloseButton   = "[data-testid='modal-close'], .modal .close, .btn-close, dialog .close";

    public ModalComponent(IPage page) : base(page) { }

    public async Task<bool> IsOpenAsync() => await IsVisibleAsync(ModalOverlay);

    public async Task WaitForOpenAsync(int timeout = 10_000) =>
        await WaitForVisibleAsync(ModalOverlay, timeout);

    public async Task<string> GetTitleAsync() => await GetTextAsync(ModalTitle);

    public async Task ClickConfirmAsync()
    {
        await ClickAsync(ConfirmButton);
        await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }

    public async Task ClickCancelAsync() => await ClickAsync(CancelButton);

    public async Task CloseAsync() => await ClickAsync(CloseButton);

    public async Task WaitForCloseAsync(int timeout = 10_000)
    {
        await Page.WaitForSelectorAsync(ModalOverlay, new PageWaitForSelectorOptions
        {
            State   = WaitForSelectorState.Hidden,
            Timeout = timeout
        });
    }
}
