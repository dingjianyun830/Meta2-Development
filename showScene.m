function showScene(MetaData,Para)
% Visualiza the camera in plot
cameraSize = 0.02;
orientRGB = Para.world2rgb(1:3,1:3);
locRGB = Para.world2rgb(1:3,4);
%locRGB(3) = -locRGB(3); 
orientPC = Para.world2depth(1:3,1:3);
locPc = Para.world2depth(1:3,4);
%locPc(3) = -locPc(3); 

M = Para.rgb2world*Para.world2rgb;

loc1 = M(1:3,4);
orient1 = M(1:3,1:3);
figure
pcshow(MetaData.ptCloud);
hold on
plotCamera('Location',[0,0,0],'Orientation',eye(3),'Size',cameraSize,'Color','r', 'Label','O','Opacity',0); 
hold on
plotCamera('Location',locRGB,'Orientation',orientRGB,'Size',cameraSize,'Color','g', 'Label','C','Opacity',0); 
hold on
plotCamera('Location',locPc,'Orientation',orientPC,'Size',cameraSize,'Color','b', 'Label','D','Opacity',0);
hold on
plotCamera('Location',loc1,'Orientation',orient1,'Size',cameraSize,'Color','b', 'Label','C1','Opacity',0); 
% hold on
% plotCamera('Location',locPc,'Orientation',orientPC,'Size',cameraSize,'Color','b', 'Label','D1','Opacity',0);
% hold on
% plotCamera('Location',locPc - M44(1:3,4),'Orientation',M44(1:3,1:3)*orientPC,'Size',cameraSize,'Color','c', 'Label','D1','Opacity',0);
% hold on
% plotCamera('Location',locPc - M44(1:3,4) - locRGB,'Orientation',M44(1:3,1:3)*orientPC,'Size',cameraSize,'Color','m', 'Label','D2','Opacity',0); 
% hold on
% plotCamera('Location',locPc - M44(1:3,4) - locRGB,'Orientation',inv(orientRGB)*M44(1:3,1:3)*orientPC,'Size',cameraSize,'Color','y', 'Label','D3','Opacity',0); 
% hold on
% plotCamera('Location',locPc-locPc+locRGB,'Orientation',orientRGB*inv(orientPC)*orientPC,'Size',cameraSize,'Color','k', 'Label','D4','Opacity',0); 
grid on
xlabel('X');
ylabel('Y');
zlabel('Z');