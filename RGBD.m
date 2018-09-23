clc
clear

folder = 'D:\shotFolder4\';
NO = '1392';

[MetaData,Para] = importMetaData(folder,NO);
%showScene(MetaData,Para);
showDepth(MetaData.depth);

x = Project(MetaData,Para);

% post-processing the image
MetaData.deIm = deformation(MetaData.im,Para.K);
%im = imrotate(im);
%im = fliplr(im);
figure
imagesc(im);
hold on
scatter(x4(1,:),x4(2,:),[], x','filled');
