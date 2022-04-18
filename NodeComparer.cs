using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrayToolbar
{
	//https://social.msdn.microsoft.com/forums/vstudio/en-US/60e5d13c-5ac3-4f26-99e8-8a6c53352112/how-to-rearrange-elements-in-a-xml-node-using-c?ranMID=46131&ranEAID=a1LgFw09t88&ranSiteID=a1LgFw09t88-r0m3XAmnXHhvshb.HgI67w&epi=a1LgFw09t88-r0m3XAmnXHhvshb.HgI67w&irgwc=1&OCID=AID2200057_aff_7806_1243925&tduid=%28ir__ol3kqmpskkkf6xg9sdldpulisu2xtcsapez0y6yq00%29%287806%29%281243925%29%28a1LgFw09t88-r0m3XAmnXHhvshb.HgI67w%29%28%29&irclickid=_ol3kqmpskkkf6xg9sdldpulisu2xtcsapez0y6yq00

	public class NodeComparer : IComparer<string>
	{
		public int Compare(string x, string y)
		{
			var xNo = int.Parse(x.Substring(x.Length - 1, 1));
			var yNo = int.Parse(y.Substring(y.Length - 1, 1));

			return xNo < yNo ? -1 : xNo == yNo ? 0 : 1;
		}
	}
}
