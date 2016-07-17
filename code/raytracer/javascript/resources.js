var lambert = function(n, l, v, params)
{
	return Math.max(0,dot(n,l));
};

var blinnPhong = function(n, l, v, params)
{
	var h = normalize(add(v, l));
	return Math.pow(Math.max(0,dot(n,h)), params.shininess);
};

var resources = {
	materials: {
		red_lambert:
		{
			type: "brdf",
			color: [1,0,0],
			brdfParams:{},
			brdf: lambert
		},
		green_lambert:
		{
			type: "brdf",
			color: [0,1,0],
			brdfParams:{},
			brdf: lambert
		},
		white_lambert:
		{
			type: "brdf",
			color: [1,1,1],
			brdfParams:{},
			brdf: lambert
		},
		white_bph_100:
		{
			type: "brdf",
			color: [1,1,1],
			brdfParams:{shininess: 100},
			brdf: blinnPhong
		},
		mirror_03:
		{
			color: [1,1,1],
			type: "mirror",
			reflectivity: 0.3
		}

	 }
};
