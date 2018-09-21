function R = RotationM(eul)
R = zeros(3,3,size(eul,1),'like',eul);
ct = cos(eul);
st = sin(eul);
%     The rotation matrix R can be constructed as follows by
%     ct = [cx cy cz] and st = [sx sy sz]
%
%     R = [  cy*cz   sy*sx*cz-sz*cx    sy*cx*cz+sz*sx
%            cy*sz   sy*sx*sz+cz*cx    sy*cx*sz-cz*sx
%              -sy            cy*sx             cy*cx]

R(1,1,:) = ct(:,2).*ct(:,1);
R(1,2,:) = st(:,3).*st(:,2).*ct(:,1) - ct(:,3).*st(:,1);
R(1,3,:) = ct(:,3).*st(:,2).*ct(:,1) + st(:,3).*st(:,1);
R(2,1,:) = ct(:,2).*st(:,1);
R(2,2,:) = st(:,3).*st(:,2).*st(:,1) + ct(:,3).*ct(:,1);
R(2,3,:) = ct(:,3).*st(:,2).*st(:,1) - st(:,3).*ct(:,1);
R(3,1,:) = -st(:,2);
R(3,2,:) = st(:,3).*ct(:,2);
R(3,3,:) = ct(:,3).*ct(:,2);