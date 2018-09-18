function [Rx,Ry,Rz] = RotationM(R)
% set the rotation(x-axis)
Rx = zeros(3,3);
Rx(1,1) = 1;
Rx(2,2) = cos(R(1));
Rx(2,3) = -sin(R(1));
Rx(3,2) = sin(R(1));
Rx(3,3) = cos(R(1));

% set the rotation(y-axis)
Ry = zeros(3,3);
Ry(2,2) = 1;
Ry(1,1) = cos(R(2));
Ry(1,3) = sin(R(2));
Ry(3,1) = -sin(R(2));
Ry(3,3) = cos(R(2));

% set the rotation(z-axis)
Rz = zeros(3,3);
Rz(3,3) = 1;
Rz(1,1) = cos(R(3));
Rz(1,2) = -sin(R(3));
Rz(2,1) = sin(R(3));
Rz(2,2) = cos(R(3));