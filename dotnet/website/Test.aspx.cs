using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Chainlink.OCRConfig;

public partial class Test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            var oracles = ChainlinkOCR.TestOracles();
            cTestJSON.Text = ChainlinkOCR.ToJson(oracles);
        }
    }

    protected void TestBtn_Click(object sender, EventArgs e)
    {
        List<OCROracleIdentity> oracles = ChainlinkOCR.FromJson<List<OCROracleIdentity>>(cTestJSON.Text);
        var s = Enumerable.Range(0, oracles.Count).Select(i => (byte)1);
        var config = ChainlinkOCR.MakeSetConfigEncodedComponents(NetworkType.Slow, oracles, 10000000 / 100, 1, s.ToArray());
        var configJson = ChainlinkOCR.ToJson(config);

    }
}