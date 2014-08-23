// datablock StaticShapeData(CubeShapeData)
// {
// 	shapeFile = $SS::Path @ "shapes/primitives/cube/cube.dts";
// };

// datablock StaticShapeData(CubeCollisionShapeData)
// {
// 	shapeFile = $SS::Path @ "shapes/primitives/cube/cube_collision.dts";
// };

datablock StaticShapeData(CubeGlowShapeData)
{
	shapeFile = $SS::Path @ "shapes/primitives/cube/cube_glow.dts";
};

// datablock StaticShapeData(CubeGlowCollisionShapeData)
// {
// 	shapeFile = $SS::Path @ "shapes/primitives/cube/cube_glow_collision.dts";
// };

// datablock StaticShapeData(ConeShapeData)
// {
// 	shapeFile = $SS::Path @ "shapes/primitives/cone/cone.dts";
// };

datablock StaticShapeData(ConeGlowShapeData)
{
	shapeFile = $SS::Path @ "shapes/primitives/cone/cone_glow.dts";
};

// datablock StaticShapeData(ConeCollisionShapeData)
// {
// 	shapeFile = $SS::Path @ "shapes/primitives/cone/cone_collision.dts";
// };

datablock StaticShapeData(CylinderGlowShapeData)
{
	shapeFile = $SS::Path @ "shapes/primitives/cylinder/cylinder_glow.dts";
};

datablock StaticShapeData(SphereGlowShapeData)
{
	shapeFile = $SS::Path @ "shapes/primitives/sphere/sphere_glow.dts";
};

//...

function createDebugLine(%a, %b, %color, %time, %size)
{
	if (%color $= "")
		%color = "1 0 0 1";

	if (%time $= "")
		%time = 1000;

	%shape = createShape(CylinderGlowShapeData, %color);
	%shape.transformLine(%a, %b, %size);
	%shape.schedule(%time, delete);

	return %shape;
}

function createDebugCube(%point, %scale, %color, %time, %size)
{
	if (%color $= "")
		%color = "1 0 0 1";

	if (%time $= "")
		%time = 1000;

	%shape = createShape(CubeGlowShapeData, %color);
	%shape.setTransform(%point);
	%shape.setScale(%scale);
	%shape.schedule(%time, delete);

	return %shape;
}

function createShape(%dataBlock, %color)
{
	%obj = new StaticShape()
	{
		datablock = %dataBlock;
	};

	MissionCleanup.add(%obj);

	if (%color !$= "")
		%obj.setNodeColor("ALL", %color);

	return %obj;
}

function StaticShape::transformLine(%this, %a, %b, %size)
{
	if (%size $= "")
	{
		%size = 0.2;
	}

	%vector = vectorNormalize(vectorSub(%b, %a));

	%xyz = vectorNormalize(vectorCross("1 0 0", %vector));
	%u = mACos(vectorDot("1 0 0", %vector)) * -1;

	%this.setTransform(vectorScale(vectorAdd(%a, %b), 0.5) SPC %xyz SPC %u);
	%this.setScale(vectorDist(%a, %b) SPC %size SPC %size);
}

function StaticShape::transformCube(%this, %transform, %size)
{
	%this.setTransform(%position);
	%this.setScale(%size SPC %size SPC %size);
}

function StaticShape::transformWorldBox(%this, %box)
{
	%neg = getWords(%box, 0, 2);
	%pos = getWords(%box, 3, 5);

	%x1 = getWord(%neg, 0);
	%y1 = getWord(%neg, 1);
	%z1 = getWord(%neg, 2);

	%x2 = getWord(%pos, 0);
	%y2 = getWord(%pos, 1);
	%z2 = getWord(%pos, 2);

	%xd = %x2 - %x1;
	%yd = %y2 - %y1;
	%zd = %z2 - %z1;

	%rot = getWords(matrixCreateFromEuler(0 SPC 0 SPC 0 SPC $pi / 2), 3, 6);

	%this.setTransform(%x1 + %xd / 2 SPC %y1 + %yd / 2 SPC %z1 + %zd / 2 SPC %rot);
	%this.setScale(%xd SPC %yd SPC %zd);
}

function eulerToAxis(%euler)
{
	%euler = VectorScale(%euler,$pi / 180);
	%matrix = MatrixCreateFromEuler(%euler);
	return getWords(%matrix,3,6);
}

function axisToEuler(%axis)
{
	%angleOver2 = getWord(%axis,3) * 0.5;
	%angleOver2 = -%angleOver2;
	%sinThetaOver2 = mSin(%angleOver2);
	%cosThetaOver2 = mCos(%angleOver2);
	%q0 = %cosThetaOver2;
	%q1 = getWord(%axis,0) * %sinThetaOver2;
	%q2 = getWord(%axis,1) * %sinThetaOver2;
	%q3 = getWord(%axis,2) * %sinThetaOver2;
	%q0q0 = %q0 * %q0;
	%q1q2 = %q1 * %q2;
	%q0q3 = %q0 * %q3;
	%q1q3 = %q1 * %q3;
	%q0q2 = %q0 * %q2;
	%q2q2 = %q2 * %q2;
	%q2q3 = %q2 * %q3;
	%q0q1 = %q0 * %q1;
	%q3q3 = %q3 * %q3;
	%m13 = 2.0 * (%q1q3 - %q0q2);
	%m21 = 2.0 * (%q1q2 - %q0q3);
	%m22 = 2.0 * %q0q0 - 1.0 + 2.0 * %q2q2;
	%m23 = 2.0 * (%q2q3 + %q0q1);
	%m33 = 2.0 * %q0q0 - 1.0 + 2.0 * %q3q3;
	return mRadToDeg(mAsin(%m23)) SPC mRadToDeg(mAtan(-%m13, %m33)) SPC mRadToDeg(mAtan(-%m21, %m22));
}