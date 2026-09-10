using OpenCvWpfTracking.Common;
using OpenCvWpfTracking.Models.Main;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace OpenCvWpfTracking.ViewModels.Main
{
    public partial class MainViewModel
    {
        private bool _isLoadingRtspCommunicationSettings;

        private void LoadRtspCommunicationSettings()
        {
            _isLoadingRtspCommunicationSettings = true;

            try
            {
                Properties.Settings settings = Properties.Settings.Default;
                string eoAddress = settings.SavedEoRtspUrl?.Trim();
                string irAddress = settings.SavedIrRtspUrl?.Trim();

                if (!IsValidRtspAddress(eoAddress)) eoAddress = MoeEoRtspAddress;
                if (!IsValidRtspAddress(irAddress)) irAddress = MoeIrRtspAddress;

                EoSourceAddress = eoAddress;
                IrSourceAddress = irAddress;
                AiRtsp0Address = eoAddress;
                AiRtsp1Address = irAddress;

                _selectedEoRtspSource = ResolveRtspSource(
                    EoRtspSourceOptions, settings.SavedEoRtspPreset, eoAddress);
                _selectedIrRtspSource = ResolveRtspSource(
                    IrRtspSourceOptions, settings.SavedIrRtspPreset, irAddress);
                _selectedAiEoRtspSource = _selectedEoRtspSource;
                _selectedAiIrRtspSource = _selectedIrRtspSource;

                OnPropertyChanged(nameof(SelectedEoRtspSource));
                OnPropertyChanged(nameof(SelectedIrRtspSource));
                OnPropertyChanged(nameof(SelectedAiEoRtspSource));
                OnPropertyChanged(nameof(SelectedAiIrRtspSource));
                OnPropertyChanged(nameof(IsEoRtspDirectInput));
                OnPropertyChanged(nameof(IsIrRtspDirectInput));
                OnPropertyChanged(nameof(IsAiEoRtspDirectInput));
                OnPropertyChanged(nameof(IsAiIrRtspDirectInput));

                ConsoleLogHelper.StateSection(
                    "RTSP CONFIG", "Saved camera settings loaded", string.Empty,
                    "EO_PRESET=" + _selectedEoRtspSource?.DisplayName,
                    "EO=" + ConsoleLogHelper.MaskRtspPassword(eoAddress),
                    "IR_PRESET=" + _selectedIrRtspSource?.DisplayName,
                    "IR=" + ConsoleLogHelper.MaskRtspPassword(irAddress));
            }
            catch (Exception ex)
            {
                EoSourceAddress = MoeEoRtspAddress;
                IrSourceAddress = MoeIrRtspAddress;
                AiRtsp0Address = EoSourceAddress;
                AiRtsp1Address = IrSourceAddress;
                ConsoleLogHelper.Error(
                    "RTSP CONFIG", "Load failed; MOE PTZ defaults retained", ex);
            }
            finally
            {
                _isLoadingRtspCommunicationSettings = false;
            }

        }

        private void SaveRtspCommunicationSettings()
        {
            if (_isLoadingRtspCommunicationSettings) return;

            try
            {
                Properties.Settings settings = Properties.Settings.Default;
                settings.SavedEoRtspUrl = EoSourceAddress?.Trim() ?? string.Empty;
                settings.SavedIrRtspUrl = IrSourceAddress?.Trim() ?? string.Empty;
                settings.SavedEoRtspPreset =
                    SelectedEoRtspSource?.DisplayName ?? "직접 입력";
                settings.SavedIrRtspPreset =
                    SelectedIrRtspSource?.DisplayName ?? "직접 입력";
                settings.Save();

                ConsoleLogHelper.StateSection(
                    "RTSP CONFIG", "Camera settings saved", string.Empty,
                    "EO_PRESET=" + settings.SavedEoRtspPreset,
                    "EO=" + ConsoleLogHelper.MaskRtspPassword(settings.SavedEoRtspUrl),
                    "IR_PRESET=" + settings.SavedIrRtspPreset,
                    "IR=" + ConsoleLogHelper.MaskRtspPassword(settings.SavedIrRtspUrl));
            }
            catch (Exception ex)
            {
                ConsoleLogHelper.Error(
                    "RTSP CONFIG", "Save failed; current runtime values retained", ex);
            }

        }

        private static RtspSourceOption ResolveRtspSource(
            System.Collections.Generic.IEnumerable<RtspSourceOption> options,
            string savedDisplayName,
            string address)
        {
            RtspSourceOption byName = options.FirstOrDefault(option =>
                string.Equals(option.DisplayName, savedDisplayName,
                    StringComparison.OrdinalIgnoreCase));

            if (byName != null &&
                (byName.IsDirectInput || string.Equals(
                    byName.Address, address, StringComparison.OrdinalIgnoreCase)))
            {
                return byName;
            }

            return options.FirstOrDefault(option =>
                       !option.IsDirectInput && string.Equals(
                           option.Address, address, StringComparison.OrdinalIgnoreCase))
                   // 제거된 GOP/MR300 프로필이 사용자 설정에 남아 있으면
                   // 임의 주소를 직접 입력으로 승계하지 않고 MOE PTZ 기본값으로 복구한다.
                   // 실제 직접 입력은 위 byName 분기에서 그대로 보존된다.
                   ?? options.First(option => !option.IsDirectInput);
        }

    }

}
