function deform_Im = deformation(im,K)
deform_Im = zeros(size(im));
for i = 1:3
    I_d = im2double(im(:,:,i));
    I_r = zeros(size(I_d));
    
    A = K(1:3,1:3);
    fx = K(1,1);
    fy = K(2,2);
    cx = K(1,3);
    cy = K(2,3);
    k1 = K(1,4);
    k2 = K(2,4);
    k3 = K(3,4);
    
    [v,u] = find(~isnan(I_r));
    XYZc=inv(A)*[u v ones(length(u),1)]';
    r2 = XYZc(1,:).^2 + XYZc(2,:).^2;
    x = XYZc(1,:);
    y = XYZc(2,:);
    
    x = x.*(1+k1*r2+k2*r2.^2+k3*r2.^3);
    y = y.*(1+k1*r2+k2*r2.^2+k3*r2.^3);
    u_d = reshape(fx*x +cx,size(I_r));
    v_d = reshape(fy*y +cy,size(I_r));
    
    I_r = interp2(I_d,u_d,v_d);
    deform_Im(:,:,i) = I_r;
end
figure
imagesc(im);
figure
imagesc(deform_Im);