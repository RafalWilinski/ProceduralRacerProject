using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Colorful/Vintage")]
public class CC_Vintage : CC_LookupFilter
{
	public enum Filter
	{
		None,
		F1977,
		Amaro,
		Brannan,
		Earlybird,
		Hefe,
		Hudson,
		Inkwell,
		Kelvin,
		LoFi,
		Mayfair,
		Nashville,
		Rise,
		Sierra,
		Sutro,
		Toaster,
		Valencia,
		Walden,
		Willow,
		XProII
	}

	public Filter filter = Filter.None;

	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (filter == Filter.None)
			lookupTexture = null;
		else
			lookupTexture = Resources.Load<Texture2D>("Instagram/" + filter.ToString());

		base.OnRenderImage(source, destination);
	}
}
