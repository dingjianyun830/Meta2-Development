clc
clear
% set the work folder
folder = 'D:\shotFolder9\';
NO = '0672';
% input the rgb frame
im = imread([folder NO 'shot.jpg']);

% input the Rotation and Trans Martix of RGB camera
[rgbR,rgbT] = importRTpara([folder NO 'shot_RT.txt']);
[Rx,Ry,Rz]=RotationM(rgbR);
R11 = Rz*Ry*Rx;
% input the intrinsics parameter of RGB camera
K = importIntrinsics([folder 'camera_Intr.txt']);

% deform the image
deform_Im = deformation(im,K);

% input the Point Cloud Data
PointCloud = importPLY([folder NO '.ply']);
% delete the point(0,0,0)
Zind = find(PointCloud(:,1)==0);
PointCloud(Zind,:) = [];
[m,n] = size(PointCloud);

% input the Rotation and Trans Martix of Depth camera
[pcR,pcT] = importRTpara([folder NO 'pc_RT.txt']);
[Rxp,Ryp,Rzp]=RotationM(pcR);
R22 = Rzp*Ryp*Rxp;
% compute the projection matrix from depth to RGB
% rgbP = eye(4);
% pcP = eye(4);
% rgbP(1:3,1:3) = R11;
% rgbP(1:3,4) = rgbT;
% pcP(1:3,1:3) = R22;
% pcP(1:3,4) = pcT;

R111 = Rz*Ry*Rx*inv(Rxp)*inv(Ryp)*inv(Rzp);
x = PointCloud';
x1 = R111*(x-pcT)+rgbT;
x2 = K(1:3,1:3)*x1;

x3 = abs(x2);

deform_Im1 = fliplr(deform_Im);
figure
imagesc(deform_Im1);


hold on
cols = jet;
for i =1:m
    col_idx = round(1/x3(3,i));
    plot(floor(x3(1,i)),floor(x3(2,i)),'o','LineWidth',4,'MarkerSize',2,'Color',cols(col_idx,:));
    hold on
end

