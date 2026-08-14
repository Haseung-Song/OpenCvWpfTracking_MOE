namespace OpenCvWpfTracking.Services.Communication.WebAgent
{
    public sealed class WebAgentThermalPaletteService
    {
        private readonly ControlCommandService _controlCommandService;

        public WebAgentThermalPaletteService(ControlCommandService controlCommandService)
        {
            _controlCommandService = controlCommandService;
        }

        public bool SelectPrevious() => _controlCommandService.SelectPreviousIrPalette();
        public bool SelectNext() => _controlCommandService.SelectNextIrPalette();
        public bool SelectBlackHot() => _controlCommandService.SelectIrBlackHotPalette();
        public bool SelectWhiteHot() => _controlCommandService.SelectIrWhiteHotPalette();
        public bool SelectRainbow() => _controlCommandService.SelectIrRainbowPalette();

        // 2026-08-14: Route the documented NUC command through the same Web Agent path.
        public bool RequestNuc() => _controlCommandService.RequestIrNuc();
    }
}
