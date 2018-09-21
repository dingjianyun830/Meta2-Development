function R = quat2rotm1(q)
% Reshape the quaternions in the depth dimension
% q2 = reshape(q,[4 1 size(q,2)]);

s = q(1,1);
x = q(1,2);
y = q(1,3);
z = q(1,4);

% Explicitly define concatenation dimension for codegen
tempR = cat(1, 1 - 2*(y.^2 + z.^2),   2*(x.*y - s.*z),   2*(x.*z + s.*y),...
2*(x.*y + s.*z), 1 - 2*(x.^2 + z.^2),   2*(y.*z - s.*x),...
2*(x.*z - s.*y),   2*(y.*z + s.*x), 1 - 2*(x.^2 + y.^2) );

R = reshape(tempR, [3, 3, length(s)]);
R = permute(R, [2 1 3]);