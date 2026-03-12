using System.Runtime.CompilerServices;

using HUtil.UI;

using Unity.Properties;

[assembly: InternalsVisibleTo("HUtil.UI.Editor")]
namespace HUtil.UI
{
    /// <summary>
    /// ViweModel 인터페이스<br />
    /// 해당 인터페이스를 구현해야 UIComponent가 정상적으로 ViewModel을 찾을 수 있습니다
    /// </summary>
    public interface IViewModel
    {

    }
}