//Checks intersection between ray and specific sphere
function intersectSphere(ray)
{
	  var a = 1.0; //dot(ray.direction, ray.direction);
	  var subPos = sub(ray.position, this.position);
	  var b = 2* dot(subPos, ray.direction);
	  var c = dot(subPos, subPos) - this.radius*this.radius;

	  var discr = b*b - 4*a*c;
	  if(discr < 0.0)
			return Infinity;

	  discr = Math.sqrt(discr);
	  var t0 = (-b - discr) / (2*a);
	  var t1 = (-b + discr) / (2*a);

	  var tMin = Math.min(t0, t1);
	  if(tMin < 0.0)
			return Infinity;

	  return tMin;
}