function x = Project(MetaData,Para)
pt = MetaData.ptCloud.Location;
idx = find(pt(:,3)==0);
pt(idx,:) = [];
[m,n] = size(pt);
pt(:,4) = ones(m,1);
M = eye(4);
M(3,3) = -1;
M1 = Para.rgb2world;
M2 = Para.world2rgb;
(M*M1*M)*(M*M2*M);

pt1 = M2*pt';
K = Para.K;
x = K(1:3,1:3)*pt1(1:3,:);
figure
scatter(x(1,:),x(2,:),[], x','filled');