These sub-graphs require the Stylized Water 3 render feature to be active and the "Height Prepass" functionality enabled on it.

If so, the water geometry's height (including any displacement effects) are rendered into a buffer.

This allows other shaders to sample the water's height information this way. May be used for various effects