using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Windows.Forms;

namespace AVMatrixController
{
    // 1. 여기서 'Form'을 'MaterialForm'으로 변경합니다. (상속 변경)
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();

            // 2. 테마 관리자(MaterialSkinManager) 인스턴스를 가져옵니다.
            var materialSkinManager = MaterialSkinManager.Instance;

            // 3. 현재 폼(Form1)을 관리자에 등록합니다.
            materialSkinManager.AddFormToManage(this);

            // 4. 테마 색상 설정 (기본값: 라이트 모드)
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            // 5. 포인트 색상 설정 (원하는 색조로 변경 가능)
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600, Primary.Blue700,
                Primary.Blue200, Accent.LightBlue200,
                TextShade.WHITE
            );
        }
    }
}