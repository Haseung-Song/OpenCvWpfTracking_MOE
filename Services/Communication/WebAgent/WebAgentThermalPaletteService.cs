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
    }
}
